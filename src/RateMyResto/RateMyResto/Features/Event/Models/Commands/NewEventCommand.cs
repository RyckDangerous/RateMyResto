namespace RateMyResto.Features.Event.Models.Commands;

/// <summary>
/// Commande pour créer un événement
/// </summary>
public sealed record NewEventCommand
{
    /// <summary>
    /// Identifiant de l'équipe
    /// </summary>
    public required Guid IdTeam { get; init; }

    /// <summary>
    /// Id de l'initiateur de l'événement
    /// </summary>
    public required int IdInitiateur { get; init; }

    /// <summary>
    /// Id du restaurant (si existant)
    /// </summary>
    public required int? IdRestaurant { get; init; }

    /// <summary>
    /// Nom du restaurant
    /// </summary>
    public required string NomRestaurant { get; init; }

    /// <summary>
    /// Adresse du restaurant
    /// </summary>
    public required string Adresse { get; init; }

    /// <summary>
    /// Lien Google Maps du restaurant
    /// </summary>
    public required string? LienGoogleMaps { get; set; }

    /// <summary>
    /// Date de l'évènement
    /// </summary>
    public required DateOnly DateEvent { get; set; }
}