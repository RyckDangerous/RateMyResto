using Microsoft.AspNetCore.Components;
using RateMyResto.Features.Event.Services;

namespace RateMyResto.Features.Event;

public partial class EventPage : ComponentBase
{
    [Inject]
    private EventViewService _viewService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        _viewService.RegisterUiRefresh(() => InvokeAsync(StateHasChanged));
        await _viewService.LoadEventsAsync();
    }
}
