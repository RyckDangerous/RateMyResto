using RateMyResto.Features.Event.Models.Commands;
using RateMyResto.Features.Event.Models.Dbs;

namespace RateMyResto.Features.Event.Repositories;

public interface IEventRepository
{
    /// <summary>
    /// Crée un événement
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<ResultOf> CreateEventAsync(NewEventCommand command);

    /// <summary>
    /// Récupère le détail d'un événement
    /// </summary>
    /// <param name="idEvent"></param>
    /// <returns></returns>
    Task<ResultOf> GetDetailEventAsync(int idEvent);

    /// <summary>
    /// Récupère la liste des événements
    /// <paramref name="userId"/></param>
    /// </summary>
    /// <returns></returns>
    Task<ResultOf<List<EventByUserDb>>> GetEventsAsync(string userId);
}