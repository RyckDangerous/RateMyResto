using RateMyResto.Features.Shared.Models;

namespace RateMyResto.Features.EventDetail.Models.ViewModels;

public sealed record ParticipantViewModel
{
    /// <summary>
    /// Nom du participant.
    /// </summary>
    public required string ParticipantName { get; init; }

    /// <summary>
    /// Statut de participation du participant.
    /// </summary>
    public required ParticipationStatus Status { get; init; }

    /// <summary>
    /// Note attribuée par le participant.
    /// </summary>
    public required decimal? Note { get; init; }

    /// <summary>
    /// Commentaire du participant.
    /// </summary>
    public required string? Commentaire { get; init; }

    /// <summary>
    /// Date de la revue.
    /// </summary>
    public required DateOnly? DateReview { get; init; }
}