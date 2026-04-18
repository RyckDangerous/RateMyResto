using System.Globalization;
using BlazorMail.Rendering;
using Microsoft.Extensions.Options;
using RateMyResto.Features.Mailing.Components;
using RateMyResto.Features.Mailing.Models;
using RateMyResto.Features.Mailing.Models.DbModels;
using RateMyResto.Features.Mailing.Repositories;

namespace RateMyResto.Features.Mailing.Services;

/// <summary>
/// Orchestration de l'envoi de la notification de note finale :
/// récupération des destinataires, rendu HTML via BlazorMail, envoi SMTP.
/// </summary>
public sealed class FinalScoreNotificationService : IFinalScoreNotificationService
{
    private const string EventDetailRoute = "/event/detail/";

    private readonly IFinalScoreNotificationRepository _repository;
    private readonly IBlazorMailRenderer _renderer;
    private readonly IMailSender _mailSender;
    private readonly MailingAppSettings _mailingSettings;
    private readonly ILogger<FinalScoreNotificationService> _logger;

    public FinalScoreNotificationService(IFinalScoreNotificationRepository repository,
                                         IBlazorMailRenderer renderer,
                                         IMailSender mailSender,
                                         IOptions<MailingAppSettings> mailingSettings,
                                         ILogger<FinalScoreNotificationService> logger)
    {
        _repository = repository;
        _renderer = renderer;
        _mailSender = mailSender;
        _mailingSettings = mailingSettings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendNotificationsAsync(FinalScoreNotificationCommand command)
    {
        _logger.LogInformation(
            "Démarrage des notifications de note finale pour l'événement {EventId}", command.EventId);

        if (string.IsNullOrWhiteSpace(_mailingSettings.AppBaseUrl))
        {
            _logger.LogWarning(
                "AppBaseUrl non configurée — notifications ignorées. Vérifier ENVRATE_Mailing__AppBaseUrl.");
            return;
        }

        ResultOf<List<FinalScoreRecipientDb>> recipientsResult =
            await _repository.GetFinalScoreRecipientsAsync(command.EventId);

        if (recipientsResult.HasError)
        {
            _logger.LogError(
                "Impossible de récupérer les destinataires pour l'événement {EventId} : {Error}",
                command.EventId, recipientsResult.Error.Message);
            return;
        }

        List<FinalScoreRecipientDb> recipients = recipientsResult.Value;

        if (recipients.Count is 0)
        {
            _logger.LogInformation(
                "Aucun destinataire pour la note finale de l'événement {EventId}", command.EventId);
            return;
        }

        _logger.LogInformation(
            "{Count} destinataire(s) à notifier pour la note finale de l'événement {EventId}",
            recipients.Count, command.EventId);

        string baseUrl        = _mailingSettings.AppBaseUrl.TrimEnd('/');
        string eventDetailUrl = $"{baseUrl}{EventDetailRoute}{command.EventId}";
        string subject        = $"🏆 Le verdict est tombé — {command.NomRestaurant} · {command.NoteGlobale}/5";

        int sentCount = 0;

        foreach (FinalScoreRecipientDb recipient in recipients)
        {
            try
            {
                string html = await _renderer.RenderAsync<FinalScoreEmailComponent>(new()
                {
                    { nameof(FinalScoreEmailComponent.DisplayName),    recipient.DisplayName },
                    { nameof(FinalScoreEmailComponent.NomRestaurant),  command.NomRestaurant },
                    { nameof(FinalScoreEmailComponent.NomEquipe),      command.NomEquipe },
                    { nameof(FinalScoreEmailComponent.DateEvenement),  command.DateEvenement },
                    { nameof(FinalScoreEmailComponent.NoteGlobale),    command.NoteGlobale },
                    { nameof(FinalScoreEmailComponent.EventDetailUrl), eventDetailUrl }
                });

                await _mailSender.SendHtmlAsync(recipient.Email, subject, html);

                _logger.LogInformation(
                    "Notification de note finale envoyée à {Email} pour l'événement {EventId}",
                    recipient.Email, command.EventId);

                sentCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Échec de la notification de note finale à {Email} pour l'événement {EventId}",
                    recipient.Email, command.EventId);
            }
        }

        _logger.LogInformation(
            "Notifications de note finale envoyées : {SentCount}/{Total} pour l'événement {EventId}",
            sentCount, recipients.Count, command.EventId);
    }
}
