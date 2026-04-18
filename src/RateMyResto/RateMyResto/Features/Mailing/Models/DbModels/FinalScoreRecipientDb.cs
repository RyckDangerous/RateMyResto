namespace RateMyResto.Features.Mailing.Models.DbModels;

/// <summary>
/// Données retournées par sp_GetFinalScoreEmailData pour chaque participant
/// confirmé à notifier lors de la publication de la note finale d'un événement.
/// </summary>
public sealed record FinalScoreRecipientDb
{
    /// <summary>
    /// Adresse email du destinataire.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Nom d'affichage du destinataire (DisplayName ou UserName en fallback).
    /// </summary>
    public required string DisplayName { get; init; }
}
