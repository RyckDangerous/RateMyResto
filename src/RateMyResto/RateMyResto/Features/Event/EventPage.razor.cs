using Microsoft.AspNetCore.Components;
using RateMyResto.Features.Event.Models.ViewModels;
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

    /// <summary>
    /// Retourne la classe CSS pour la bordure de la carte en fonction du statut
    /// </summary>
    private string GetCardStatusClass(ParticipationStatus status)
    {
        return status switch
        {
            ParticipationStatus.Invite => "card-status-invite",
            ParticipationStatus.Confirme => "card-status-confirme",
            ParticipationStatus.Decline => "card-status-decline",
            ParticipationStatus.Absent => "card-status-absent",
            _ => ""
        };
    }

    /// <summary>
    /// Retourne la classe CSS pour le badge de date en fonction du statut
    /// </summary>
    private string GetDateBadgeClass(ParticipationStatus status)
    {
        return status switch
        {
            ParticipationStatus.Invite => "badge-invite",
            ParticipationStatus.Confirme => "badge-confirme",
            ParticipationStatus.Decline => "badge-decline",
            ParticipationStatus.Absent => "badge-absent",
            _ => ""
        };
    }
}
