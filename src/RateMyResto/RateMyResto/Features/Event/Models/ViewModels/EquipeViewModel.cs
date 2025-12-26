namespace RateMyResto.Features.Event.Models.ViewModels;

public sealed record EquipeViewModel
{
    /// <summary>
    /// Identifiant unique de l'équipe.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Nom de l'équipe.
    /// </summary>
    public required string Nom { get; init; }
}
