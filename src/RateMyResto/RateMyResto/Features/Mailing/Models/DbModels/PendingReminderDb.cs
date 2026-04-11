namespace RateMyResto.Features.Mailing.Models.DbModels;

/// <summary>
/// Donnée brute issue de sp_GetPendingVoteReminders : un rappel de vote
/// à envoyer pour un participant confirmé d'un évènement terminé sans note.
/// </summary>
public sealed record PendingReminderDb
{
    /// <summary>
    /// Identifiant de l'évènement concerné.
    /// </summary>
    public required Guid EventId { get; init; }

    /// <summary>
    /// Date de l'évènement.
    /// </summary>
    public required DateOnly DateEvenement { get; init; }

    /// <summary>
    /// Nom du restaurant de l'évènement.
    /// </summary>
    public required string NomRestaurant { get; init; }

    /// <summary>
    /// Identifiant du lien utilisateur-équipe (dbo.UserTeams.Id).
    /// </summary>
    public required int UserTeamsId { get; init; }

    /// <summary>
    /// Identifiant AspNetUsers de l'utilisateur.
    /// </summary>
    public required string AspNetUserId { get; init; }

    /// <summary>
    /// Adresse email du destinataire.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Nom d'affichage (DisplayName ou UserName en fallback).
    /// </summary>
    public required string DisplayName { get; init; }
}
