namespace RateMyResto.Features.Team.Models;

public sealed record TeamCommand
{
    /// <summary>
    /// Identifiant de l'équipe
    /// </summary>
    public required Guid IdTeam { get; init; }

    /// <summary>
    /// Nom du membre de l'équipe
    /// </summary>
    public required string Nom { get; init; }

    /// <summary>
    /// Description de l'équipe
    /// </summary>
    public required string? Description { get; init; }

    /// <summary>
    /// Identifiant du propriétaire de l'équipe
    /// </summary>
    public required Guid Owner { get; init; }
}
