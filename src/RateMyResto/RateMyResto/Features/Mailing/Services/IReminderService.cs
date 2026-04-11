namespace RateMyResto.Features.Mailing.Services;

/// <summary>
/// Service d'orchestration de l'envoi des rappels de vote.
/// </summary>
public interface IReminderService
{
    /// <summary>
    /// Récupère les rappels en attente puis envoie un email à chaque participant.
    /// Chaque envoi réussi est tracé en base pour garantir l'idempotence.
    /// </summary>
    /// <returns>Le nombre de rappels effectivement envoyés.</returns>
    Task<int> SendPendingRemindersAsync();
}
