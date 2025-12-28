namespace RateMyResto.Features.EventDetail.Models.InputModels;

public sealed record EventRatingInput
{
    /// <summary>
    /// La note donnée par l'utilisateur (de 0 à 5).
    /// </summary>
    public decimal? Rating { get; set; }

    /// <summary>
    /// Le commentaire optionnel de l'utilisateur.
    /// </summary>
    public string? Comment { get; set; }
}
