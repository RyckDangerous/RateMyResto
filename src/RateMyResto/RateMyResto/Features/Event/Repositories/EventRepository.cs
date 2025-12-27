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
            GetSqlParameterNVarchar("@UserId", userId)
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
            GetSqlParameterUniqueIdentifier("@TeamId", command.IdTeam),
            GetSqlParameterInt("@InitiateurId", command.IdInitiateur),
            GetSqlParameterInt("@RestaurantId", command.IdRestaurant),
            GetSqlParameterDate("@DateEvenement", command.DateEvent)
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

    /// <inheritdoc />
    public async Task<ResultOf> UpdateParticipationStatusAsync(UpdateStatusCommand command)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterNVarchar("@UserId", command.UserId),
            GetSqlParameterInt("@EventId", command.EventId),
            GetSqlParameterTinyInt("@StatusParticipationId", command.Status)
        };

        return await ExecuteStoredProcedureAsync(procName: "sp_UpdateParticipationStatus",
                                                parameters: parameters);

    }
}
