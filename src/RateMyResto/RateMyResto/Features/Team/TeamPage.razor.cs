using Microsoft.AspNetCore.Components;
using RateMyResto.Features.Team.Services;

namespace RateMyResto.Features.Team;

public partial class TeamPage : ComponentBase
{

    private readonly ITeamViewService _viewService;


    public TeamPage(ITeamViewService viewService)
    {
        _viewService = viewService;
    }


    protected override async Task OnInitializedAsync()
    {
        await _viewService.LoadViewModelAsync();
    }
}
