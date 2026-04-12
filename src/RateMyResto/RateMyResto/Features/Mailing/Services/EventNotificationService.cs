using System.Globalization;
using BlazorMail.Rendering;
using Microsoft.Extensions.Options;
using RateMyResto.Features.Mailing.Components;
using RateMyResto.Features.Mailing.Models;
using RateMyResto.Features.Mailing.Models.DbModels;
using RateMyResto.Features.Mailing.Repositories;

namespace RateMyResto.Features.Mailing.Services;

/// <summary>
/// Orchestration de l'envoi des notifications lors de la création d'un événement :
/// récupération des destinataires, rendu HTML via BlazorMail, envoi SMTP.
/// </summary>
public sealed class EventNotificationService : IEventNotificationService
{
    private const string TeamEventsRoute = "/events";

    private readonly IEventNotificationRepository _repository;
    private readonly IBlazorMailRenderer _renderer;
    private readonly IMailSender _mailSender;
    private readonly MailingAppSettings _mailingSettings;
    private readonly ILogger<EventNotificationService> _logger;

    public EventNotificationService(IEventNotificationRepository repository,
                                    IBlazorMailRenderer renderer,
                                    IMailSender mailSender,
                                    IOptions<MailingAppSettings> mailingSettings,
                                    ILogger<EventNotificationService> logger)
    {
        _repository = repository;
        _renderer = renderer;
        _mailSender = mailSender;
        _mailingSettings = mailingSettings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<int> SendNewEventNotificationsAsync(Guid eventId)
    {
        _logger.LogInformation("Démarrage des notifications pour le nouvel événement {EventId}", eventId);

        if (string.IsNullOrWhiteSpace(_mailingSettings.AppBaseUrl))
        {
            _logger.LogWarning(
                "AppBaseUrl non configurée — notifications ignorées. Vérifier ENVRATE_Mailing__AppBaseUrl.");
            return 0;
        }

        ResultOf<List<NewEventParticipantDb>> recipientsResult =
            await _repository.GetNewEventRecipientsAsync(eventId);

        if (recipientsResult.HasError)
        {
            _logger.LogError("Impossible de récupérer les destinataires pour l'événement {EventId} : {Error}",
                eventId, recipientsResult.Error.Message);
            return 0;
        }

        List<NewEventParticipantDb> recipients = recipientsResult.Value;

        if (recipients.Count is 0)
        {
            _logger.LogInformation("Aucun destinataire pour l'événement {EventId}", eventId);
            return 0;
        }

        _logger.LogInformation("{Count} destinataire(s) à notifier pour l'événement {EventId}",
            recipients.Count, eventId);

        string baseUrl = _mailingSettings.AppBaseUrl.TrimEnd('/');
        string teamEventsUrl = $"{baseUrl}{TeamEventsRoute}";
        int sentCount = 0;

        // Le sujet est commun à tous les destinataires d'un même événement
        NewEventParticipantDb first = recipients[0];
        string formattedDate = first.DateEvenement.ToString("dddd d MMMM yyyy",
                                                            CultureInfo.GetCultureInfo("fr-FR"));
        string subject = $"RateMyResto — Nouvel événement chez {first.NomRestaurant} le {formattedDate}";

        foreach (NewEventParticipantDb recipient in recipients)
        {
            try
            {
                string html = await _renderer.RenderAsync<NewEventEmailComponent>(new()
                {
                    { nameof(NewEventEmailComponent.DisplayName),      recipient.DisplayName },
                    { nameof(NewEventEmailComponent.NomOrganisateur),  recipient.NomOrganisateur },
                    { nameof(NewEventEmailComponent.NomEquipe),        recipient.NomEquipe },
                    { nameof(NewEventEmailComponent.NomRestaurant),    recipient.NomRestaurant },
                    { nameof(NewEventEmailComponent.AdresseRestaurant), recipient.AdresseRestaurant },
                    { nameof(NewEventEmailComponent.DateEvenement),    recipient.DateEvenement },
                    { nameof(NewEventEmailComponent.TeamEventsUrl),    teamEventsUrl }
                });

                await _mailSender.SendHtmlAsync(recipient.Email, subject, html);

                _logger.LogInformation("Notification envoyée à {Email} pour l'événement {EventId}",
                    recipient.Email, eventId);

                sentCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Échec de la notification à {Email} pour l'événement {EventId}",
                    recipient.Email, eventId);
            }
        }

        _logger.LogInformation("Notifications envoyées : {SentCount}/{Total} pour l'événement {EventId}",
            sentCount, recipients.Count, eventId);

        return sentCount;
    }
}
