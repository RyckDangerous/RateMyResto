using RateMyResto.Features.EventDetail.Models;

namespace RateMyResto.Features.EventDetail.Services;

public interface IEventDetailViewService
{
    /// <summary>
    /// Le ViewModel des détails de l'événement.
    /// </summary>
    EventDetailViewModel? ViewModel { get; }

    /// <summary>
    /// Charge les détails de l'événement.
    /// </summary>
    /// <param name="idEvent"></param>
    /// <returns></returns>
    Task LoadEvent(Guid idEvent);

    /// <summary>
    /// Soumet la notation d'un utilisateur pour l'événement.
    /// </summary>
    /// <param name="eventId">ID de l'événement</param>
    /// <param name="rating">Note entre 0 et 5</param>
    /// <param name="comment">Commentaire optionnel</param>
    /// <returns></returns>
    Task SubmitRatingAsync(Guid eventId, decimal rating, string? comment);
}