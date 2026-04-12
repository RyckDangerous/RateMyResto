using Microsoft.Data.SqlClient;
using RateMyResto.Core.Data;
using RateMyResto.Features.HomeFeature.RestoDetail.Models;
using RateMyResto.Features.Shared.Configurations;

namespace RateMyResto.Features.HomeFeature.RestoDetail.Repositories;

/// <summary>
/// Implémentation du repository pour la page de détail public d'un événement.
/// </summary>
public sealed class RestoDetailRepository : RepositoryBase<RestoDetailRepository>, IRestoDetailRepository
{
    public RestoDetailRepository(IApplicationSettings config, ILogger<RestoDetailRepository> logger)
        : base(config, logger)
    {
    }

    /// <inheritdoc />
    public async Task<ResultOf<PublicEventDetailDb>> GetEventDetailAsync(Guid eventId)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterUniqueIdentifier("@EventId", eventId)
        };

        return await ExecuteStoredProcedureWithJsonResultAsync<PublicEventDetailDb>(
            procName: "sp_GetEventDetailPublic",
            parameters: parameters);
    }
}
