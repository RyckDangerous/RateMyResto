using Microsoft.Data.SqlClient;
using RateMyResto.Core.Data;
using RateMyResto.Features.Mailing.Models.DbModels;
using RateMyResto.Features.Shared.Configurations;

namespace RateMyResto.Features.Mailing.Repositories;

/// <summary>
/// Implémentation du repository pour les notifications de note finale.
/// </summary>
public sealed class FinalScoreNotificationRepository : RepositoryBase<FinalScoreNotificationRepository>, IFinalScoreNotificationRepository
{
    public FinalScoreNotificationRepository(IApplicationSettings config,
                                            ILogger<FinalScoreNotificationRepository> logger)
        : base(config, logger)
    {
    }

    /// <inheritdoc />
    public async Task<ResultOf<List<FinalScoreRecipientDb>>> GetFinalScoreRecipientsAsync(Guid eventId)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterUniqueIdentifier("@EventRepasId", eventId)
        };

        ResultOf<List<FinalScoreRecipientDb>> result =
            await ExecuteStoredProcedureWithJsonResultAsync<List<FinalScoreRecipientDb>>(
                procName: "sp_GetFinalScoreEmailData",
                parameters: parameters);

        if (result.HasError && result.Error is NotFoundError)
        {
            return ResultOf.Success(new List<FinalScoreRecipientDb>());
        }

        return result;
    }
}
