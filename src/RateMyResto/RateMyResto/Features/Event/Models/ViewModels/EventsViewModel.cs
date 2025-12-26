namespace RateMyResto.Features.Event.Models.ViewModels;

/// <summary>
/// Modèle pour les événements
/// </summary>
public sealed record EventsViewModel
{
    /// <summary>
    /// Liste des événements par équipe
    /// </summary>
    public required IEnumerable<EventByTeamViewModel> EventsByTeam { get; set; }
}
