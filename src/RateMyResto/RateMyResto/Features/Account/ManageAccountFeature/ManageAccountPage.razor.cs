using Microsoft.AspNetCore.Components;
using RateMyResto.Features.Account.ManageAccountFeature.Services;
using RateMyResto.Features.Shared.Services;

namespace RateMyResto.Features.Account.ManageAccountFeature;

public partial class ManageAccountPage : ComponentBase
{
    [Inject]
    private IManageAccountViewService _viewService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (_viewService is IViewServiceBase viewServiceBase)
        {
            viewServiceBase.RegisterUiRefresh(() => InvokeAsync(StateHasChanged));
        }

        await _viewService.LoadUsersAsync();
        StateHasChanged();
    }
}
