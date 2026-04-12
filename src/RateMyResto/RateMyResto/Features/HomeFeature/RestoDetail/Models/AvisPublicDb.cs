namespace RateMyResto.Features.HomeFeature.RestoDetail.Models;

/// <summary>
/// Avis anonymisé d'un participant, issu de sp_GetEventDetailPublic.
/// Aucun nom n'est exposé.
/// </summary>
public sealed record AvisPublicDb
{
    /// <summary>
    /// Note attribuée par le participant (0–5).
    /// </summary>
    public required decimal Note { get; init; }

    /// <summary>
    /// Commentaire laissé par le participant. Peut être nul.
    /// </summary>
    public string? Commentaire { get; init; }

    /// <summary>
    /// Date de publication de l'avis.
    /// </summary>
    public required DateOnly DateReview { get; init; }
}
