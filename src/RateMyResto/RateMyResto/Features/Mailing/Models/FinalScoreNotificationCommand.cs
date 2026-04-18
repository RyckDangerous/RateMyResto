namespace RateMyResto.Features.Mailing.Models;

/// <summary>
/// Données nécessaires à l'envoi de la notification de note finale.
/// </summary>
public sealed record FinalScoreNotificationCommand
{
    /// <summary>
    /// Identifiant de l'événement dont la note finale vient d'être calculée.
    /// </summary>
    public required Guid EventId { get; init; }

    /// <summary>
    /// Nom du restaurant de l'événement.
    /// </summary>
    public required string NomRestaurant { get; init; }

    /// <summary>
    /// Nom de l'équipe organisatrice.
    /// </summary>
    public required string NomEquipe { get; init; }

    /// <summary>
    /// Date de l'événement.
    /// </summary>
    public required DateOnly DateEvenement { get; init; }

    /// <summary>
    /// Note globale calculée (moyenne des participants confirmés, arrondie à 2 décimales).
    /// </summary>
    public required decimal NoteGlobale { get; init; }
}
