using BlazorMail.Rendering;
using RateMyResto.Features.Mailing.Components;
using RateMyResto.Features.Mailing.Models.DbModels;
using RateMyResto.Features.Mailing.Repositories;

namespace RateMyResto.Features.Mailing.Services;

/// <summary>
/// Orchestration du mode CLI "--reminder" : récupération, rendu HTML via BlazorMail,
/// envoi SMTP et trace de chaque envoi réussi.
/// </summary>
public sealed class ReminderService : IReminderService
{
    private const string EmailSubject = "RateMyResto — Pensez à noter votre dernier repas";

    private readonly IReminderRepository _repository;
    private readonly IBlazorMailRenderer _renderer;
    private readonly IMailSender _mailSender;
    private readonly ILogger<ReminderService> _logger;

    public ReminderService(IReminderRepository repository,
                           IBlazorMailRenderer renderer,
                           IMailSender mailSender,
                           ILogger<ReminderService> logger)
    {
        _repository = repository;
        _renderer = renderer;
        _mailSender = mailSender;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<int> SendPendingRemindersAsync()
    {
        _logger.LogInformation("Démarrage de l'envoi des rappels de vote");

        ResultOf<List<PendingReminderDb>> pendingResult = await _repository.GetPendingRemindersAsync();

        if (pendingResult.HasError)
        {
            _logger.LogError("Impossible de récupérer les rappels en attente : {Error}",
                pendingResult.Error.Message);
            return 0;
        }

        List<PendingReminderDb> pending = pendingResult.Value;

        if (pending.Count is 0)
        {
            _logger.LogInformation("Aucun rappel à envoyer");
            return 0;
        }

        _logger.LogInformation("{Count} rappel(s) à envoyer", pending.Count);

        int sentCount = 0;

        foreach (PendingReminderDb reminder in pending)
        {
            try
            {
                string html = await _renderer.RenderAsync<VoteReminderEmailComponent>(new()
                {
                    { nameof(VoteReminderEmailComponent.DisplayName),   reminder.DisplayName },
                    { nameof(VoteReminderEmailComponent.NomRestaurant), reminder.NomRestaurant },
                    { nameof(VoteReminderEmailComponent.DateEvenement), reminder.DateEvenement }
                });

                await _mailSender.SendHtmlAsync(reminder.Email, EmailSubject, html);

                ResultOf markResult = await _repository.MarkReminderSentAsync(
                    reminder.EventId, reminder.UserTeamsId, DateTime.UtcNow);

                if (markResult.HasError)
                {
                    _logger.LogWarning(
                        "Rappel envoyé à {Email} mais impossible de le tracer en base : {Error}",
                        reminder.Email, markResult.Error.Message);
                }

                sentCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Échec de l'envoi du rappel à {Email} pour l'évènement {EventId}",
                    reminder.Email, reminder.EventId);
            }
        }

        _logger.LogInformation("Rappels envoyés : {SentCount}/{Total}", sentCount, pending.Count);

        return sentCount;
    }
}
