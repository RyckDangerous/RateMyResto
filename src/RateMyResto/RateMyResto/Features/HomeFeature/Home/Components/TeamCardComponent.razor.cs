using Microsoft.AspNetCore.Components;
using RateMyResto.Features.HomeFeature.Home.Models;

namespace RateMyResto.Features.HomeFeature.Home.Components;

/// <summary>
/// Card d'équipe pour la page d'accueil publique.
/// Affiche le nom, la description et les statistiques de l'équipe.
/// Autonome : aucun service injecté.
/// </summary>
public sealed partial class TeamCardComponent : ComponentBase
{
    /// <summary>
    /// Données de l'équipe à afficher.
    /// </summary>
    [Parameter]
    public required TeamPublicViewModel Team { get; set; }

    /// <summary>
    /// Callback déclenché quand l'utilisateur clique sur la card.
    /// Transmet l'identifiant de l'équipe au composant parent.
    /// </summary>
    [Parameter]
    public EventCallback<Guid> OnTeamClicked { get; set; }

    /// <summary>
    /// Transmet l'identifiant de l'équipe au parent via OnTeamClicked.
    /// </summary>
    private async Task HandleClick()
    {
        if (OnTeamClicked.HasDelegate)
        {
            await OnTeamClicked.InvokeAsync(Team.Id);
        }
    }
}
