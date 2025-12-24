namespace RateMyResto.Features.Team.Models;

/// <summary>
/// Représente une équipe avec son propriétaire et ses membres dans la base de données.
/// </summary>
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
    public required string OwnerId { get; init; }

    /// <summary>
    /// Nom du propriétaire de l'équipe.
    /// </summary>
    public required string OwnerName { get; set; }

    /// <summary>
    /// Liste des membres de l'équipe.
    /// </summary>
    public required List<TeamMemberDb> Members { get; set; }
}
