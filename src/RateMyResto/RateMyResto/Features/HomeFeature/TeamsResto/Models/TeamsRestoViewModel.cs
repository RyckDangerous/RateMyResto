namespace RateMyResto.Features.HomeFeature.TeamsResto.Models;

/// <summary>
/// ViewModel pour la page des sorties restaurant d'une équipe.
/// </summary>
public sealed record TeamsRestoViewModel
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
    /// </summary>
    public required IReadOnlyList<EventPublicDb> Evenements { get; init; }
}
