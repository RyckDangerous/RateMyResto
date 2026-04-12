using RateMyResto.Features.Mailing.Models.DbModels;

namespace RateMyResto.Features.Mailing.Repositories;

/// <summary>
/// Contrat d'accès aux données pour les notifications de création d'événement.
/// </summary>
public interface IEventNotificationRepository
{
    /// <summary>
    /// Retourne la liste des destinataires à notifier pour un événement nouvellement créé.
    /// Seuls les membres actifs (DateFinPresence IS NULL) disposant d'une adresse email sont inclus.
    /// </summary>
    /// <param name="eventId">Identifiant de l'événement.</param>
    /// <returns>Liste des participants avec les informations nécessaires à l'envoi du mail.</returns>
    Task<ResultOf<List<NewEventParticipantDb>>> GetNewEventRecipientsAsync(Guid eventId);
}
