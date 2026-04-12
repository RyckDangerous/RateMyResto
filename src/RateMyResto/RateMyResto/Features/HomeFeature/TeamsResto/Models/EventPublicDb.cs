namespace RateMyResto.Features.HomeFeature.TeamsResto.Models;

/// <summary>
/// Données brutes d'un événement passé, imbriquées dans TeamEventsPublicDb.
/// </summary>
public sealed record EventPublicDb
{
    /// <summary>
    /// Identifiant unique de l'événement.
    /// </summary>
    public required Guid EventId { get; init; }

    /// <summary>
    /// Nom du restaurant de la sortie.
    /// </summary>
    public required string NomRestaurant { get; init; }

    /// <summary>
    /// Adresse du restaurant.
    /// </summary>
    public required string Adresse { get; init; }

    /// <summary>
    /// Date de la sortie.
    /// </summary>
    public required DateOnly DateEvenement { get; init; }

    /// <summary>
    /// Note globale de l'événement. Nulle si aucun vote enregistré.
    /// </summary>
    public decimal? NoteGlobale { get; init; }
}
