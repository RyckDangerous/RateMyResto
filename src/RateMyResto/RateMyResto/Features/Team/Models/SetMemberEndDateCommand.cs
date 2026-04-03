namespace RateMyResto.Features.Team.Models;

/// <summary>
/// Commande pour définir ou effacer la date de fin de présence d'un membre dans une équipe.
/// </summary>
public sealed record SetMemberEndDateCommand
{
    /// <summary>
    /// Identifiant de l'équipe.
    /// </summary>
    public required Guid TeamId { get; init; }

    /// <summary>
    /// Identifiant ASP.NET Identity du membre.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Date de fin de présence. Null pour réactiver le membre.
    /// </summary>
    public required DateOnly? EndDate { get; init; }
}
