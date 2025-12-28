namespace RateMyResto.Features.EventDetail.Models.Commands;

public sealed record RatingCommand
{
    /// <summary>
    /// L'Id de l'événement à évaluer.
    /// </summary>
    public Guid EventId { get; init; }

    /// <summary>
    /// L'Id de l'utilisateur qui évalue l'événement.
    /// </summary>
    public string UserId { get; init; }

    /// <summary>
    /// La note attribuée à l'événement.
    /// </summary>
    public decimal Rating { get; init; }

    /// <summary>
    /// Le commentaire associé à la note.
    /// </summary>
    public string? Comment { get; init; }
}
