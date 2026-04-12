using System.Globalization;
using Microsoft.AspNetCore.Components;
using RateMyResto.Features.HomeFeature.RestoDetail.ViewServices;

namespace RateMyResto.Features.HomeFeature.RestoDetail;

/// <summary>
/// Page publique de détail d'un événement restaurant.
/// Affiche photos, note globale et avis anonymisés — aucun nom de participant.
/// Accessible sans authentification.
/// </summary>
public sealed partial class RestoDetailPage : ComponentBase
{
    [Inject]
    private IRestoDetailViewService _viewService { get; set; } = default!;

    /// <summary>
    /// Identifiant de l'équipe (utilisé pour le lien retour).
    /// </summary>
    [Parameter]
    public Guid TeamId { get; set; }

    /// <summary>
    /// Identifiant de l'événement passé en paramètre de route.
    /// </summary>
    [Parameter]
    public Guid EventId { get; set; }

    /// <summary>
    /// Date de l'événement formatée en français pour l'affichage.
    /// </summary>
    private string FormattedDate => _viewService.ViewModel is not null
        ? _viewService.ViewModel.DateEvenement.ToString("dddd d MMMM yyyy", CultureInfo.GetCultureInfo("fr-FR"))
        : string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await _viewService.LoadAsync(EventId);
    }
}
