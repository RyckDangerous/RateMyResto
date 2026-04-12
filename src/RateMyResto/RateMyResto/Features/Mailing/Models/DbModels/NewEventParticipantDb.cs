namespace RateMyResto.Features.Mailing.Models.DbModels;

/// <summary>
/// Données retournées par sp_GetNewEventEmailData pour chaque membre de l'équipe
/// à notifier lors de la création d'un nouvel événement.
/// </summary>
public sealed record NewEventParticipantDb
{
    /// <summary>
    /// Adresse email du destinataire.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Nom d'affichage du destinataire (DisplayName ou UserName en fallback).
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Nom d'affichage de l'organisateur de l'événement.
    /// </summary>
    public required string NomOrganisateur { get; init; }

    /// <summary>
    /// Nom de l'équipe organisatrice.
    /// </summary>
    public required string NomEquipe { get; init; }

    /// <summary>
    /// Nom du restaurant de l'événement.
    /// </summary>
    public required string NomRestaurant { get; init; }

    /// <summary>
    /// Adresse du restaurant.
    /// </summary>
    public required string AdresseRestaurant { get; init; }

    /// <summary>
    /// Date de l'événement.
    /// </summary>
    public required DateOnly DateEvenement { get; init; }
}
