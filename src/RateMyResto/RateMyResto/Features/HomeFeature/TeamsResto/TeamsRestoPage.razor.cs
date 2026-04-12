using Microsoft.AspNetCore.Components;
using RateMyResto.Features.HomeFeature.TeamsResto.ViewServices;

namespace RateMyResto.Features.HomeFeature.TeamsResto;

/// <summary>
/// Page publique listant les sorties restaurant passées d'une équipe.
/// Accessible sans authentification.
/// </summary>
public sealed partial class TeamsRestoPage : ComponentBase
{
    [Inject]
    private ITeamsRestoViewService _viewService { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    /// <summary>
    /// Identifiant de l'équipe passé en paramètre de route.
    /// </summary>
    [Parameter]
    public Guid TeamId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await _viewService.LoadAsync(TeamId);
    }

    /// <summary>
    /// Navigue vers le détail d'un événement.
    /// </summary>
    /// <param name="eventId">Identifiant de l'événement.</param>
    public void HandleEventClicked(Guid eventId)
    {
        Navigation.NavigateTo($"/equipe/{TeamId}/evenement/{eventId}");
    }
}
