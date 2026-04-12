namespace RateMyResto.Features.HomeFeature.RestoDetail.Models;

/// <summary>
/// Données brutes issues de sp_GetEventDetailPublic :
/// détail complet d'un événement avec avis anonymisés.
/// </summary>
public sealed record PublicEventDetailDb
{
    /// <summary>
    /// Identifiant de l'événement.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Date de l'événement.
    /// </summary>
    public required DateOnly DateEvenement { get; init; }

    /// <summary>
    /// Note globale de l'événement. Nulle si non encore calculée.
    /// </summary>
    public decimal? NoteGlobale { get; init; }

    /// <summary>
    /// Nom du restaurant.
    /// </summary>
    public required string NomRestaurant { get; init; }

    /// <summary>
    /// Adresse du restaurant.
    /// </summary>
    public required string Adresse { get; init; }

    /// <summary>
    /// Lien Google Maps vers le restaurant. Peut être nul.
    /// </summary>
    public string? LienGoogleMaps { get; init; }

    /// <summary>
    /// Nom de l'équipe organisatrice.
    /// </summary>
    public required string NomEquipe { get; init; }

    /// <summary>
    /// Identifiant de l'équipe (pour le lien de retour).
    /// </summary>
    public required Guid TeamId { get; init; }

    /// <summary>
    /// Liste des avis anonymisés des participants confirmés.
    /// Nulle si aucun avis.
    /// </summary>
    public IReadOnlyList<AvisPublicDb>? Avis { get; init; }
}
