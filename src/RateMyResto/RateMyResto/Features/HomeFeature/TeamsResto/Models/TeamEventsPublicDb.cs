namespace RateMyResto.Features.HomeFeature.TeamsResto.Models;

/// <summary>
/// Données brutes issues de sp_GetTeamEventsPublic :
/// informations de l'équipe avec la liste de ses événements passés.
/// </summary>
public sealed record TeamEventsPublicDb
{
    /// <summary>
    /// Identifiant de l'équipe.
    /// </summary>
    public required Guid TeamId { get; init; }

    /// <summary>
    /// Nom de l'équipe.
    /// </summary>
    public required string NomEquipe { get; init; }

    /// <summary>
    /// Description de l'équipe. Peut être nulle.
    /// </summary>
    public string? DescriptionEquipe { get; init; }

    /// <summary>
    /// Liste des événements passés de l'équipe.
    /// Nulle si l'équipe n'a aucun événement passé.
    /// </summary>
    public IReadOnlyList<EventPublicDb>? Evenements { get; init; }
}
