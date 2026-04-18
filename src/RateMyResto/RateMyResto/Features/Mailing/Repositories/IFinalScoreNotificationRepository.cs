using RateMyResto.Features.Mailing.Models.DbModels;

namespace RateMyResto.Features.Mailing.Repositories;

/// <summary>
/// Contrat d'accès aux données pour les notifications de note finale.
/// </summary>
public interface IFinalScoreNotificationRepository
{
    /// <summary>
    /// Retourne la liste des participants confirmés à notifier pour la note finale d'un événement.
    /// Seuls les membres actifs (DateFinPresence IS NULL) disposant d'une adresse email sont inclus.
    /// </summary>
    /// <param name="eventId">Identifiant de l'événement.</param>
    /// <returns>Liste des destinataires avec email et nom d'affichage.</returns>
    Task<ResultOf<List<FinalScoreRecipientDb>>> GetFinalScoreRecipientsAsync(Guid eventId);
}
