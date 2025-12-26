namespace RateMyResto.Features.Event.Models.Queries;

public sealed record UserQuery
{

    /// <summary>
    /// Identifiant de l'utilisateur
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Identifiant de l'équipe
    /// </summary>
    public required Guid TeamId { get; set; }
}
