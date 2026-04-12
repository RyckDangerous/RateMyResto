using RateMyResto.Features.HomeFeature.Home.Models;

namespace RateMyResto.Features.HomeFeature.Home.Repositories;

/// <summary>
/// Contrat d'accès aux données pour la page d'accueil publique.
/// </summary>
public interface IHomeRepository
{
    /// <summary>
    /// Récupère toutes les équipes avec leurs statistiques publiques.
    /// </summary>
    /// <returns>Liste des équipes, vide si aucune.</returns>
    Task<ResultOf<List<TeamPublicDb>>> GetAllTeamsAsync();
}
