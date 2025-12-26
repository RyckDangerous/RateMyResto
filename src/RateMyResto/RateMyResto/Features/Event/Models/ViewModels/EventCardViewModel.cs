namespace RateMyResto.Features.Event.Models.ViewModels;

/// <summary>
/// Modèle pour une carte d'événement
/// </summary>
public sealed record EventCardViewModel
{
    /// <summary>
    /// Identifiant de l'événement
    /// </summary>
    public required int IdEvent { get; init; }

    /// <summary>
    /// Nom du restaurant
    /// </summary>
    public required string NomDuRestaurant { get; init; }

    /// <summary>
    /// Date de l'événement
    /// </summary>
    public required DateOnly DateEvent { get; init; }

    /// <summary>
    /// Statut de participation de l'utilisateur à l'événement
    /// </summary>
    public required ParticipationStatus Status { get; set; }
}
