using RateMyResto.Features.Event.Models.ViewModels;

namespace RateMyResto.Features.Event.Models.Commands;

public sealed record UpdateStatusCommand
{
    /// <summary>
    /// Id de l'utilisateur
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Id de l'événement
    /// </summary>
    public required int EventId { get; init; }

    /// <summary>
    /// Nouveau statut de l'événement
    /// </summary>
    public required short Status { get; init; }
}
