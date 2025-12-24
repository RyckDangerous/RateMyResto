using Microsoft.AspNetCore.Components;
using RateMyResto.Features.Team.Services;

namespace RateMyResto.Features.Team;

public partial class TeamPage : ComponentBase
{
    [Inject]
    private ITeamViewService _viewService { get; set; } = default!;

    // État de la modale
    private bool _showCreateModal = false;
    private string _newTeamName = string.Empty;
    private string? _newTeamDescription = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await _viewService.LoadViewModelAsync();
    }

    private void OpenCreateModal()
    {
        _showCreateModal = true;
        _newTeamName = string.Empty;
        _newTeamDescription = string.Empty;
    }

    private void CloseCreateModal()
    {
        _showCreateModal = false;
        _newTeamName = string.Empty;
        _newTeamDescription = string.Empty;
    }

    /// <summary>
    /// Gère la création d'une nouvelle équipe.
    /// </summary>
    /// <returns></returns>
    private async Task HandleCreateTeam()
    {
        if (string.IsNullOrWhiteSpace(_newTeamName))
        {
            return;
        }

        await _viewService.CreateTeamAsync(_newTeamName, _newTeamDescription);
        CloseCreateModal();
        StateHasChanged();
    }

    /// <summary>
    /// Gère le départ d'une équipe.
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    private async Task HandleLeaveTeam(Guid teamId)
    {
        await _viewService.LeaveTeamAsync(teamId);
        StateHasChanged();
    }
}
