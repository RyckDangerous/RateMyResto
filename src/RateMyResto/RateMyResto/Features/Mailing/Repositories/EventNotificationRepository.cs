using Microsoft.Data.SqlClient;
using RateMyResto.Core.Data;
using RateMyResto.Features.Mailing.Models.DbModels;
using RateMyResto.Features.Shared.Configurations;

namespace RateMyResto.Features.Mailing.Repositories;

/// <summary>
/// Implémentation du repository pour les notifications de création d'événement.
/// </summary>
public sealed class EventNotificationRepository : RepositoryBase<EventNotificationRepository>, IEventNotificationRepository
{
    public EventNotificationRepository(IApplicationSettings config, ILogger<EventNotificationRepository> logger)
        : base(config, logger)
    {
    }

    /// <inheritdoc />
    public async Task<ResultOf<List<NewEventParticipantDb>>> GetNewEventRecipientsAsync(Guid eventId)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterUniqueIdentifier("@EventRepasId", eventId)
        };

        ResultOf<List<NewEventParticipantDb>> result =
            await ExecuteStoredProcedureWithJsonResultAsync<List<NewEventParticipantDb>>(
                procName: "sp_GetNewEventEmailData",
                parameters: parameters);

        if (result.HasError && result.Error is NotFoundError)
        {
            return ResultOf.Success(new List<NewEventParticipantDb>());
        }

        return result;
    }
}
