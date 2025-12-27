using Microsoft.Data.SqlClient;
using RateMyResto.Core.Data;
using RateMyResto.Features.EventDetail.Models;
using RateMyResto.Features.Shared.Configurations;

namespace RateMyResto.Features.EventDetail.Repositories;

public sealed class EventDetailRepository : RepositoryBase<EventDetailRepository>, IEventDetailRepository
{

    public EventDetailRepository(IApplicationSettings config, ILogger<EventDetailRepository> logger)
        : base(config, logger)
    {
    }

    /// <inheritdoc />
    public async Task<ResultOf<EventDetailDb>> GetDetailEventAsync(Guid idEvent)
    {
        SqlParameter[] sqlParameters =
        {
            GetSqlParameterUniqueIdentifier("@IdEvent", idEvent)
        };

        return await ExecuteStoredProcedureWithJsonResultAsync<EventDetailDb>(procName: "sp_GetEventById",
                                                                            parameters: sqlParameters);
    }
}
