using Microsoft.AspNetCore.Components;
using RateMyResto.Features.Shared.Models;

namespace RateMyResto.Features.Team.Components;

public partial class TeamDrawerContent : ComponentBase
{
    /// <summary>
    /// Équipe à afficher
    /// </summary>
    [Parameter]
    public required Equipe Team { get; set; }

    /// <summary>
    /// Indique si l'utilisateur est le propriétaire de l'équipe
    /// </summary>
    [Parameter]
    public bool IsOwner { get; set; } = false;

    /// <summary>
    /// Callback quand l'utilisateur veut supprimer l'équipe
    /// </summary>
    [Parameter]
    public EventCallback<Guid> OnDeleteTeam { get; set; }

    /// <summary>
    /// Callback quand l'utilisateur veut retirer un membre
    /// </summary>
    [Parameter]
    public EventCallback<string> OnRemoveMember { get; set; }

    /// <summary>
    /// Callback quand l'utilisateur veut désactiver un membre
    /// </summary>
    [Parameter]
    public EventCallback<string> OnDeactivateMember { get; set; }

    /// <summary>
    /// Callback quand l'utilisateur veut réactiver un membre
    /// </summary>
    [Parameter]
    public EventCallback<string> OnReactivateMember { get; set; }

    private async Task HandleDeleteTeam()
    {
        DrawerService.Close();
        if (OnDeleteTeam.HasDelegate)
        {
            await OnDeleteTeam.InvokeAsync(Team.Id);
        }
    }

    private async Task HandleRemoveMember(string userId)
    {
        if (OnRemoveMember.HasDelegate)
        {
            await OnRemoveMember.InvokeAsync(userId);
        }
    }

    private async Task HandleDeactivateMember(string userId)
    {
        if (OnDeactivateMember.HasDelegate)
        {
            await OnDeactivateMember.InvokeAsync(userId);
        }
    }

    private async Task HandleReactivateMember(string userId)
    {
        if (OnReactivateMember.HasDelegate)
        {
            await OnReactivateMember.InvokeAsync(userId);
        }
    }
}
