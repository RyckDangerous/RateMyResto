using Microsoft.Data.SqlClient;
using RateMyResto.Core.Data;
using RateMyResto.Features.Mailing.Models.DbModels;
using RateMyResto.Features.Shared.Configurations;

namespace RateMyResto.Features.Mailing.Repositories;

/// <summary>
/// Implémentation du repository pour la feature rappels de vote.
/// </summary>
public sealed class ReminderRepository : RepositoryBase<ReminderRepository>, IReminderRepository
{
    public ReminderRepository(IApplicationSettings config, ILogger<ReminderRepository> logger)
        : base(config, logger)
    {
    }

    /// <inheritdoc />
    public async Task<ResultOf<List<PendingReminderDb>>> GetPendingRemindersAsync()
    {
        ResultOf<List<PendingReminderDb>> result =
            await ExecuteStoredProcedureWithJsonResultAsync<List<PendingReminderDb>>(
                procName: "sp_GetPendingVoteReminders");

        if (result.HasError && result.Error is NotFoundError)
        {
            return ResultOf.Success(new List<PendingReminderDb>());
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultOf> MarkReminderSentAsync(Guid eventId, int userTeamsId, DateTime sentAt)
    {
        SqlParameter[] sqlParameters =
        {
            GetSqlParameterUniqueIdentifier("@EventRepasId", eventId),
            GetSqlParameterInt("@UserTeamsId", userTeamsId),
            GetSqlParameterDateTime2("@SentAt", sentAt)
        };

        return await ExecuteNonQueryStoredProcedureAsync(procName: "sp_MarkReminderSent",
                                                         parameters: sqlParameters);
    }
}
