using RateMyResto.Features.Shared.Models;

namespace RateMyResto.Features.EventDetail.Models.ViewModels;

public sealed record ParticipantViewModel
{
    /// <summary>
    /// Identifiant ASP.NET Identity du participant.
    /// </summary>
    public required string UserId { get; init; }

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

    /// <summary>
    /// Indique si la note doit être masquée.
    /// </summary>
    public bool HideNote { get; set; }

    /// <summary>
    /// Indique si un gestionnaire peut modifier le statut de ce participant.
    /// Vrai si aucun vote n'a encore eu lieu, ou si l'événement est terminé et que ce participant n'a pas voté.
    /// </summary>
    public bool CanChangeStatus { get; set; }

    /// <summary>
    /// Indique si ce participant est l'utilisateur actuellement connecté.
    /// </summary>
    public bool IsCurrentUser { get; set; }
}