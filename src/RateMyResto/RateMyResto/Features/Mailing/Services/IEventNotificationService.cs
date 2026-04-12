namespace RateMyResto.Features.Mailing.Services;

/// <summary>
/// Contrat du service d'envoi des notifications de création d'événement.
/// </summary>
public interface IEventNotificationService
{
    /// <summary>
    /// Envoie un email de notification à tous les membres actifs de l'équipe
    /// pour un événement nouvellement créé.
    /// </summary>
    /// <param name="eventId">Identifiant de l'événement nouvellement créé.</param>
    /// <returns>Nombre d'emails envoyés avec succès.</returns>
    Task<int> SendNewEventNotificationsAsync(Guid eventId);
}
