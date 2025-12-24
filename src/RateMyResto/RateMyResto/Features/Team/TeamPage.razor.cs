using Microsoft.AspNetCore.Components;
using RateMyResto.Features.Team.Services;

namespace RateMyResto.Features.Team;

public partial class TeamPage : ComponentBase
{
    [Inject]
    private ITeamViewService _viewService { get; set; } = default!;


    protected override async Task OnInitializedAsync()
    {
        _viewService.RegisterUiRefresh(() => InvokeAsync(StateHasChanged));

        await _viewService.LoadViewModelAsync();
    }

}
