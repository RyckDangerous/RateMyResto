namespace RateMyResto.Features.EventDetail.Models.ViewModels;

public sealed record EventDetailViewModel
{
    /// <summary>
    /// Information sur l'équipe.
    /// </summary>
    public required EquipeViewModel EquipeInfo { get; init; }

    /// <summary>
    /// Information sur le restaurant.
    /// </summary>
    public required RestaurantViewModel RestaurantInfo { get; init; }

    /// <summary>
    /// Informations sur les participants.
    /// </summary>
    public required List<ParticipantViewModel> ParticipantsInfo { get; init; }

    /// <summary>
    /// Détails de l'événement.
    /// </summary>
    public required DateOnly DateEvenement { get; init; }

    /// <summary>
    /// Nom de l'initiateur de l'événement.
    /// </summary>
    public required string NomIniateur { get; init; }

    /// <summary>
    /// Photos de l'événement.
    /// </summary>
    public required IEnumerable<string> Photos { get; init; }

    /// <summary>
    /// Note globale de l'événement.
    /// </summary>
    public required decimal? NoteGlobale { get; init; }
}
