using RateMyResto.Features.EventDetail.Models.Commands;
using RateMyResto.Features.EventDetail.Models.DbModels;

namespace RateMyResto.Features.EventDetail.Repositories;

public interface IEventDetailRepository
{
    /// <summary>
    /// Récupère le détail d'un événement
    /// </summary>
    /// <param name="idEvent"></param>
    /// <returns></returns>
    Task<ResultOf<EventDetailDb>> GetDetailEventAsync(Guid idEvent);

    /// <summary>
    /// Sauvegarde une note pour un événement
    /// </summary>
    /// <param name="ratingCommand"></param>
    /// <returns></returns>
    Task<ResultOf> SaveRatingAsync(RatingCommand ratingCommand);
}