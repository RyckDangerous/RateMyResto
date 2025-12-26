namespace RateMyResto.Features.Event.Models.ViewModels;

public sealed record EventByTeamViewModel
{
    /// <summary>
    /// Identifiant de l'équipe
    /// </summary>
    public required Guid IdEquipe { get; init; }

    /// <summary>
    /// Nom de l'équipe
    /// </summary>
    public required string TeamName { get; init; }

    /// <summary>
    /// Liste des événements
    /// </summary>
    public required List<EventCardViewModel> Events { get; set; }
}