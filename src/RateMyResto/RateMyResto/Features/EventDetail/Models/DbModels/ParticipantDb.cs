namespace RateMyResto.Features.EventDetail.Models.DbModels;

/// <summary>
/// Représente un participant à un événement avec ses informations et son éventuelle évaluation.
/// </summary>
public sealed record ParticipantDb
{
    /// <summary>
    /// Identifiant interne UserTeams du participant.
    /// </summary>
    public required int IdUser { get; init; }

    /// <summary>
    /// Identifiant ASP.NET Identity (AspNetUsers.Id) du participant.
    /// Utilisé pour les mises à jour de statut par un gestionnaire.
    /// </summary>
    public required string AspNetUserId { get; init; }

    /// <summary>
    /// Nom d'utilisateur affiché pour le participant.
    /// </summary>
    public required string UserName { get; init; }

    /// <summary>
    /// Nom d'affichage du participant.
    /// </summary>
    public required string? DisplayName { get; init; }
           
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
