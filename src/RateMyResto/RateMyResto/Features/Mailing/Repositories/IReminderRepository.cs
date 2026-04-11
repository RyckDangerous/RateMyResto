using RateMyResto.Features.Mailing.Models.DbModels;

namespace RateMyResto.Features.Mailing.Repositories;

/// <summary>
/// Accès aux données pour la feature rappels de vote.
/// </summary>
public interface IReminderRepository
{
    /// <summary>
    /// Récupère la liste des rappels de vote en attente d'envoi.
    /// </summary>
    /// <returns>La liste des rappels à envoyer (vide si aucun).</returns>
    Task<ResultOf<List<PendingReminderDb>>> GetPendingRemindersAsync();

    /// <summary>
    /// Enregistre qu'un rappel a été envoyé pour un couple (évènement, participant).
    /// </summary>
    /// <param name="eventId">Identifiant de l'évènement.</param>
    /// <param name="userTeamsId">Identifiant du lien utilisateur-équipe.</param>
    /// <param name="sentAt">Date et heure d'envoi du rappel.</param>
    Task<ResultOf> MarkReminderSentAsync(Guid eventId, int userTeamsId, DateTime sentAt);
}
