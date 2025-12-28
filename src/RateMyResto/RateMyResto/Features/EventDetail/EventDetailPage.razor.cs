using Microsoft.AspNetCore.Components;
using RateMyResto.Features.EventDetail.Services;
using RateMyResto.Features.Shared.Models;
using RateMyResto.Features.Shared.Services;

namespace RateMyResto.Features.EventDetail;

public partial class EventDetailPage : ComponentBase
{
    [Parameter]
    public Guid EventId { get; set; }

    [Inject]
    private IEventDetailViewService _viewService { get; set; } = default!;

    [Inject]
    private Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    //private decimal? _userRating;
    //private string? _userComment;
    private string? _currentUserName;

    /// <summary>
    /// Initialisation du composant.
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        // Enregistrer la fonction de rafraîchissement UI
        if (_viewService is IViewServiceBase viewServiceBase)
        {
            viewServiceBase.RegisterUiRefresh(() => InvokeAsync(StateHasChanged));
        }

        // Récupérer le nom de l'utilisateur connecté
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _currentUserName = authState.User.Identity?.Name;

        // Charger les détails de l'événement
        await _viewService.LoadEvent(EventId);
        StateHasChanged();
    }

    /// <summary>
    /// Gère la soumission de l'évaluation par l'utilisateur.
    /// </summary>
    /// <returns></returns>
    private async Task HandleSubmitRating()
    {
        if (!_viewService.RatingInput.Rating.HasValue 
            || _viewService.RatingInput.Rating < 0 
            || _viewService.RatingInput.Rating > 5)
        {
            return;
        }

        await _viewService.SubmitRatingAsync();

        // Réinitialiser les champs
        _viewService.RatingInput.Rating = null;
        _viewService.RatingInput.Comment = null;
        
        StateHasChanged();
    }

    /// <summary>
    /// Obtient la classe d'icône pour un statut de participation donné.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    private static string GetParticipantStatusIcon(ParticipationStatus status)
    {
        return status switch
        {
            ParticipationStatus.Confirme => "bi bi-check-circle-fill text-success",
            ParticipationStatus.Decline => "bi bi-x-circle-fill text-danger",
            ParticipationStatus.Absent => "bi bi-dash-circle-fill text-secondary",
            ParticipationStatus.Invite => "bi bi-question-circle-fill text-warning",
            _ => "bi bi-circle text-muted"
        };
    }

    /// <summary>
    /// Obtient la classe CSS pour une carte de participant en fonction de son statut de participation.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    private static string GetParticipantCardClass(ParticipationStatus status)
    {
        return status switch
        {
            ParticipationStatus.Confirme => "border-success bg-light-success",
            ParticipationStatus.Decline => "border-danger bg-light-danger",
            ParticipationStatus.Absent => "border-secondary bg-light",
            ParticipationStatus.Invite => "border-warning bg-light-warning",
            _ => ""
        };
    }

    /// <summary>
    /// Obtient le texte descriptif pour un statut de participation donné.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    private static string GetStatusText(ParticipationStatus status)
    {
        return status switch
        {
            ParticipationStatus.Confirme => "Présent",
            ParticipationStatus.Decline => "Absent (décliné)",
            ParticipationStatus.Absent => "Absent",
            ParticipationStatus.Invite => "Invité",
            _ => "Inconnu"
        };
    }

    /// <summary>
    /// Obtient le texte de qualité en fonction de la note globale.
    /// </summary>
    /// <param name="rating"></param>
    /// <returns></returns>
    private static string GetRatingQuality(decimal rating)
    {
        return rating switch
        {
            >= 4.5m => "Excellente sortie ! 🎉",
            >= 4.0m => "Très bonne expérience ! 😊",
            >= 3.5m => "Bonne sortie ! 👍",
            >= 3.0m => "Expérience correcte",
            >= 2.5m => "Moyenne",
            >= 2.0m => "Décevant",
            _ => "À éviter"
        };
    }
}
