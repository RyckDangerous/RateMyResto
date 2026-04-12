using Microsoft.AspNetCore.Components;
using RateMyResto.Features.HomeFeature.TeamsResto.Models;

namespace RateMyResto.Features.HomeFeature.TeamsResto.Components;

/// <summary>
/// Card d'un événement passé pour la page des sorties d'une équipe.
/// Affiche le restaurant, la date et la note globale.
/// Autonome : aucun service injecté.
/// </summary>
public sealed partial class EventCardComponent : ComponentBase
{
    /// <summary>
    /// Données de l'événement à afficher.
    /// </summary>
    [Parameter]
    public required EventPublicDb Event { get; set; }

    /// <summary>
    /// Identifiant de l'équipe (fourni par la page parente pour construire l'URL de navigation).
    /// </summary>
    [Parameter]
    public Guid TeamId { get; set; }

    /// <summary>
    /// Callback déclenché quand l'utilisateur clique sur la card.
    /// Transmet l'identifiant de l'événement au composant parent.
    /// </summary>
    [Parameter]
    public EventCallback<Guid> OnEventClicked { get; set; }

    /// <summary>
    /// Transmet l'identifiant de l'événement au parent via OnEventClicked.
    /// </summary>
    private async Task HandleClick()
    {
        if (OnEventClicked.HasDelegate)
        {
            await OnEventClicked.InvokeAsync(Event.EventId);
        }
    }
}
