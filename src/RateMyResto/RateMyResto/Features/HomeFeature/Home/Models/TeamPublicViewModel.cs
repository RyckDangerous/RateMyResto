namespace RateMyResto.Features.HomeFeature.Home.Models;

/// <summary>
/// ViewModel d'une équipe pour la page d'accueil publique.
/// </summary>
public sealed record TeamPublicViewModel
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
    /// Nombre de membres actifs de l'équipe.
    /// </summary>
    public required int NombreMembres { get; init; }

    /// <summary>
    /// Nombre de sorties restaurant passées.
    /// </summary>
    public required int NombreEvenements { get; init; }
}
