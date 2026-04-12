using RateMyResto.Features.HomeFeature.TeamsResto.Models;

namespace RateMyResto.Features.HomeFeature.TeamsResto.ViewServices;

/// <summary>
/// Contrat du service de vue pour la page des sorties d'une équipe.
/// </summary>
public interface ITeamsRestoViewService
{
    /// <summary>
    /// ViewModel chargé. Nul avant le chargement ou si l'équipe est introuvable.
    /// </summary>
    TeamsRestoViewModel? ViewModel { get; }

    /// <summary>
    /// Charge les événements passés de l'équipe identifiée par <paramref name="teamId"/>.
    /// </summary>
    /// <param name="teamId">Identifiant de l'équipe.</param>
    Task LoadAsync(Guid teamId);
}
