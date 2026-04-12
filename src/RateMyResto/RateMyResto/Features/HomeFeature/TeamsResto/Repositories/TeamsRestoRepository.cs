using Microsoft.Data.SqlClient;
using RateMyResto.Core.Data;
using RateMyResto.Features.HomeFeature.TeamsResto.Models;
using RateMyResto.Features.Shared.Configurations;

namespace RateMyResto.Features.HomeFeature.TeamsResto.Repositories;

/// <summary>
/// Implémentation du repository pour la page des sorties d'une équipe.
/// </summary>
public sealed class TeamsRestoRepository : RepositoryBase<TeamsRestoRepository>, ITeamsRestoRepository
{
    public TeamsRestoRepository(IApplicationSettings config, ILogger<TeamsRestoRepository> logger)
        : base(config, logger)
    {
    }

    /// <inheritdoc />
    public async Task<ResultOf<TeamEventsPublicDb>> GetTeamEventsAsync(Guid teamId)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterUniqueIdentifier("@TeamId", teamId)
        };

        return await ExecuteStoredProcedureWithJsonResultAsync<TeamEventsPublicDb>(
            procName: "sp_GetTeamEventsPublic",
            parameters: parameters);
    }
}
