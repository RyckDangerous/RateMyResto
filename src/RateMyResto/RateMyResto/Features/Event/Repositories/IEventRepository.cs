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
    /// Récupère la liste des événements
    /// <paramref name="userId"/></param>
    /// </summary>
    /// <returns></returns>
    Task<ResultOf<List<EventByUserDb>>> GetEventsAsync(string userId);

    /// <summary>
    /// Met à jour le statut de participation d'un utilisateur à un événement
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<ResultOf> UpdateParticipationStatusAsync(UpdateStatusCommand command);
}