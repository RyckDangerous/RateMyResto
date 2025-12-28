namespace RateMyResto.Features.EventDetail.Models.Commands;

public sealed record GlobalRatingCommand
{
    /// <summary>
    /// L'Id de l'événement à noter.
    /// </summary>
    public required Guid EventId { get; init; }

    /// <summary>
    /// La note globale à attribuer.
    /// </summary>
    public required decimal Rating { get; init; }
}
