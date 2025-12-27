
using RateMyResto.Features.EventDetail.Models;

namespace RateMyResto.Features.EventDetail.Repositories;

public interface IEventDetailRepository
{
    /// <summary>
    /// Récupère le détail d'un événement
    /// </summary>
    /// <param name="idEvent"></param>
    /// <returns></returns>
    Task<ResultOf<EventDetailDb>> GetDetailEventAsync(Guid idEvent);
}