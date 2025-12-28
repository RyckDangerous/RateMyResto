namespace RateMyResto.Features.EventDetail.Models.ViewModels;

public sealed record EquipeViewModel
{
    /// <summary>
    /// Nom de l'équipe.
    /// </summary>
    public required string TeamName { get; init; }

}
