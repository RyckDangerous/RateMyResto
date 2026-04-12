namespace RateMyResto.Features.HomeFeature.Home.Models;

/// <summary>
/// Données brutes issues de sp_GetAllTeamsPublic :
/// une équipe avec son nombre de membres actifs et de sorties passées.
/// </summary>
public sealed record TeamPublicDb
{
    /// <summary>
    /// Identifiant unique de l'équipe.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Nom de l'équipe.
    /// </summary>
    public required string Nom { get; init; }

    /// <summary>
    /// Description de l'équipe. Peut être nulle.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Nombre de membres actifs (sans DateFinPresence).
    /// </summary>
    public required int NombreMembres { get; init; }

    /// <summary>
    /// Nombre d'événements passés de l'équipe.
    /// </summary>
    public required int NombreEvenements { get; init; }
}
