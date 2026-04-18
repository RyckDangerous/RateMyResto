using RateMyResto.Features.Mailing.Models;

namespace RateMyResto.Features.Mailing.Services;

/// <summary>
/// Contrat d'envoi de la notification de note finale.
/// </summary>
public interface IFinalScoreNotificationService
{
    /// <summary>
    /// Envoie un email à tous les participants confirmés de l'événement
    /// pour leur communiquer la note finale officielle.
    /// </summary>
    /// <param name="command">Données de l'événement et note finale.</param>
    Task SendNotificationsAsync(FinalScoreNotificationCommand command);
}
