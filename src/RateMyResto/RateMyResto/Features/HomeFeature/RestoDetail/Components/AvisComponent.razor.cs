using Microsoft.AspNetCore.Components;
using RateMyResto.Features.HomeFeature.RestoDetail.Models;

namespace RateMyResto.Features.HomeFeature.RestoDetail.Components;

/// <summary>
/// Affiche un avis anonymisé (note + commentaire).
/// Aucun nom de participant n'est exposé.
/// Autonome : aucun service injecté.
/// </summary>
public sealed partial class AvisComponent : ComponentBase
{
    /// <summary>
    /// Données de l'avis à afficher.
    /// </summary>
    [Parameter]
    public required AvisPublicDb Avis { get; set; }
}
