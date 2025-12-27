namespace RateMyResto.Features.EventDetail.Models;

/// <summary>
/// Représente un participant à un événement avec ses informations et son éventuelle évaluation.
/// </summary>
public sealed record ParticipantDb
{
    /// <summary>
    /// Identifiant unique de l'utilisateur participant.
    /// </summary>
    public required int IdUser { get; init; }
           
    /// <summary>
    /// Nom d'utilisateur affiché pour le participant.
    /// </summary>
    public required string UserName { get; init; }
           
    /// <summary>
    /// Note attribuée par le participant (si disponible).
    /// </summary>
    public required decimal? Note { get; init; }
           
    /// <summary>
    /// Commentaire laissé par le participant (si disponible).
    /// </summary>
    public required string? Commentaire { get; init; }
           
    /// <summary>
    /// Date de publication de l'avis (si disponible).
    /// </summary>
    public required DateOnly? DateReview { get; init; }
           
    /// <summary>
    /// Identifiant du statut du participant (ex. confirmé, annulé, etc.).
    /// </summary>
    public required short StatusId { get; init; }
}
