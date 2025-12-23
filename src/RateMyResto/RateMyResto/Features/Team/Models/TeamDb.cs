namespace RateMyResto.Features.Team.Models;

public sealed record TeamDb
{
    /// <summary>
    /// Identifiant unique de l'équipe.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Nom de l'équipe.
    /// </summary>
    public required string Nom { get; init; }

    /// <summary>
    /// Description de l'équipe.
    /// </summary>
    public required string? Description { get; init; }

    /// <summary>
    /// Identifiant unique du propriétaire de l'équipe.
    /// </summary>
    public required Guid OwnerId { get; init; }

    /// <summary>
    /// Liste des membres de l'équipe.
    /// </summary>
    public required List<TeamMemberDb> Members { get; set; }
}
