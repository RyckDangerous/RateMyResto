namespace RateMyResto.Features.EventDetail.Models.Commands;

/// <summary>
/// Commande pour modifier le statut de participation d'un participant
/// par l'initiateur de l'événement ou le responsable de l'équipe.
/// </summary>
public sealed record ChangeParticipantStatusCommand
{
    /// <summary>
    /// Identifiant de l'événement.
    /// </summary>
    public required Guid EventId { get; init; }

    /// <summary>
    /// Identifiant ASP.NET Identity du participant dont on change le statut.
    /// </summary>
    public required string ParticipantUserId { get; init; }

    /// <summary>
    /// Nouveau statut de participation.
    /// </summary>
    public required short NewStatus { get; init; }
}
