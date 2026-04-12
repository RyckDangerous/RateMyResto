using RateMyResto.Features.HomeFeature.RestoDetail.Models;

namespace RateMyResto.Features.HomeFeature.RestoDetail.ViewServices;

/// <summary>
/// Contrat du service de vue pour la page de détail public d'un événement.
/// </summary>
public interface IRestoDetailViewService
{
    /// <summary>
    /// ViewModel chargé. Nul avant le chargement ou si l'événement est introuvable.
    /// </summary>
    RestoDetailViewModel? ViewModel { get; }

    /// <summary>
    /// Charge le détail public de l'événement identifié par <paramref name="eventId"/>.
    /// </summary>
    /// <param name="eventId">Identifiant de l'événement.</param>
    Task LoadAsync(Guid eventId);
}
