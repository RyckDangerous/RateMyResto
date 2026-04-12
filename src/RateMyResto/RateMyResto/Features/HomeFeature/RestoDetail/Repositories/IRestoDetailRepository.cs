using RateMyResto.Features.HomeFeature.RestoDetail.Models;

namespace RateMyResto.Features.HomeFeature.RestoDetail.Repositories;

/// <summary>
/// Contrat d'accès aux données pour la page de détail public d'un événement.
/// </summary>
public interface IRestoDetailRepository
{
    /// <summary>
    /// Récupère le détail public d'un événement avec ses avis anonymisés.
    /// </summary>
    /// <param name="eventId">Identifiant de l'événement.</param>
    /// <returns>Détail de l'événement.</returns>
    Task<ResultOf<PublicEventDetailDb>> GetEventDetailAsync(Guid eventId);
}
