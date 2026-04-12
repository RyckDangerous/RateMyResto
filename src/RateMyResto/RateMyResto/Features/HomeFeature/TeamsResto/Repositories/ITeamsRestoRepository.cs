using RateMyResto.Features.HomeFeature.TeamsResto.Models;

namespace RateMyResto.Features.HomeFeature.TeamsResto.Repositories;

/// <summary>
/// Contrat d'accès aux données pour la page des sorties d'une équipe.
/// </summary>
public interface ITeamsRestoRepository
{
    /// <summary>
    /// Récupère une équipe avec la liste de ses événements passés.
    /// </summary>
    /// <param name="teamId">Identifiant de l'équipe.</param>
    /// <returns>Données de l'équipe et de ses événements.</returns>
    Task<ResultOf<TeamEventsPublicDb>> GetTeamEventsAsync(Guid teamId);
}
