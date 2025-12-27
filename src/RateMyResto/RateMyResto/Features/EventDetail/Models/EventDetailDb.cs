namespace RateMyResto.Features.EventDetail.Models;

/// <summary>
/// Représente le détail complet d'un événement de restaurant, incluant
/// les informations générales, l'équipe, l'initiateur et les participants.
/// </summary>
public sealed record EventDetailDb
{
    /// <summary>
    /// Identifiant unique de l'événement.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Date de l'événement.
    /// </summary>
    public required DateOnly DateEvenement { get; init; }

    /// <summary>
    /// Nom du restaurant où se déroule l'événement.
    /// </summary>
    public required string NomRestaurant { get; init; }

    /// <summary>
    /// Adresse du restaurant.
    /// </summary>
    public required string Adresse { get; init; }

    /// <summary>
    /// Lien Google Maps vers l'emplacement du restaurant.
    /// Peut être nul si non fourni.
    /// </summary>
    public required string? LienGoogleMaps { get; init; }

    /// <summary>
    /// Nom de l'équipe participant à l'événement.
    /// </summary>
    public required string NomEquipe { get; init; }

    /// <summary>
    /// Informations sur les participants à l'événement.
    /// Liste en lecture seule.
    /// </summary>
    public required IReadOnlyList<ParticipantDb> InfoParticipants { get; init; }

    /// <summary>
    /// Nom de la personne qui a initié l'événement.
    /// </summary>
    public required string Initiateur { get; init; }
}
