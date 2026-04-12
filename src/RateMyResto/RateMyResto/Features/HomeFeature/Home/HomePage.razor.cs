using Microsoft.AspNetCore.Components;
using RateMyResto.Features.HomeFeature.Home.ViewServices;

namespace RateMyResto.Features.HomeFeature.Home;

/// <summary>
/// Page d'accueil publique. Affiche la liste des équipes en cards.
/// Accessible sans authentification.
/// </summary>
public sealed partial class HomePage : ComponentBase
{
    [Inject]
    private IHomeViewService _viewService { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await _viewService.LoadTeamsAsync();
    }

    /// <summary>
    /// Navigue vers la page des sorties de l'équipe sélectionnée.
    /// </summary>
    /// <param name="teamId">Identifiant de l'équipe.</param>
    public void HandleTeamClicked(Guid teamId)
    {
        Navigation.NavigateTo($"/equipe/{teamId}");
    }
}
