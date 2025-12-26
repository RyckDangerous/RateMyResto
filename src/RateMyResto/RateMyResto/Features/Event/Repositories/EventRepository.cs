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
        SqlParameter[] parameters =
        {
            new SqlParameter("@UserId", userId)
        };

        ResultOf<List<EventByUserDb>> results = await ExecuteStoredProcedureWithJsonResultAsync<List<EventByUserDb>>(procName: "sp_GetEventsByUser",
                                                                                                                    parameters: parameters);
        // JSON deserialization for type 'RateMyResto.Features.Event.Models.Dbs.EventByUserDb'
        // was missing required properties including: 'ParticipationStatus'; 'IdEquipe'; 'EquipeName'.
        if (results.HasError)
        {
            _logger.LogError("Erreur sur la récupérer des évènements sur l'utilisateur {UserId}: {ErrorMessage}", userId, results.Error?.Message);
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<ResultOf> CreateEventAsync(NewEventCommand command)
    {
        // sp_CreateEvent
        SqlParameter[] parameters =
        {
            new SqlParameter("@TeamId", command.IdTeam),
            new SqlParameter("@InitiateurId", command.IdInitiateur),
            new SqlParameter("@RestaurantId", (object?)command.IdRestaurant ?? DBNull.Value),
            new SqlParameter("@DateEvenement", command.DateEvent)
        };

        return await ExecuteStoredProcedureAsync(procName: "sp_CreateEvent",
                                                parameters: parameters);
    }

    /// <inheritdoc />
    public async Task<ResultOf> GetDetailEventAsync(int idEvent)
    {
        // Implementation for retrieving events
        return ResultOf.Success();
    }
}
