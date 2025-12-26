using Microsoft.Data.SqlClient;
using RateMyResto.Core.Data;
using RateMyResto.Features.Event.Models.Commands;
using RateMyResto.Features.Event.Models.Dbs;
using RateMyResto.Features.Shared.Configurations;

namespace RateMyResto.Features.Event.Repositories;

public sealed class EventRepository : RepositoryBase<EventRepository>, IEventRepository
{

    public EventRepository(IApplicationSettings config, ILogger<EventRepository> logger)
        : base(config, logger)
    {
    }

    /// <inheritdoc />
    public async Task<ResultOf<List<EventByUserDb>>> GetEventsAsync(string userId)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@UserId", userId)
        };

        ResultOf<List<EventByUserDb>> results = await ExecuteStoredProcedureWithJsonResultAsync<List<EventByUserDb>>(procName: "sp_GetEventsByUser",
                                                                                                                    parameters: parameters);

        if (results.HasError)
        {
            _logger.LogError("Erreur sur la récupérer des évènements sur l'utilisateur {UserId}: {ErrorMessage}", userId, results.Error?.Message);
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<ResultOf> CreateEventAsync(NewEventCommand command)
    {
        // Implementation for creating an event
        // sp_CreateEvent


        return ResultOf.Success();
    }

    /// <inheritdoc />
    public async Task<ResultOf> GetDetailEventAsync(int idEvent)
    {
        // Implementation for retrieving events
        return ResultOf.Success();
    }
}
