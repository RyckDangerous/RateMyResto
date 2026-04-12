using RateMyResto.Features.HomeFeature.Home.Models;

namespace RateMyResto.Features.HomeFeature.Home.ViewServices;

/// <summary>
/// Contrat du service de vue pour la page d'accueil publique.
/// </summary>
public interface IHomeViewService
{
    /// <summary>
    /// Liste des équipes à afficher. Vide avant le chargement.
    /// </summary>
    List<TeamPublicViewModel> Teams { get; }

    /// <summary>
    /// Charge la liste des équipes depuis le repository.
    /// </summary>
    Task LoadTeamsAsync();
}
