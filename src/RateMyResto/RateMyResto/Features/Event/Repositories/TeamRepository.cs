using Microsoft.Data.SqlClient;
using RateMyResto.Core.Data;
using RateMyResto.Features.Event.Models.Dbs;
using RateMyResto.Features.Event.Models.Queries;
using RateMyResto.Features.Shared.Configurations;

namespace RateMyResto.Features.Event.Repositories;

public sealed class TeamRepository : RepositoryBase<TeamRepository>, ITeamRepository
{
    public TeamRepository(IApplicationSettings config, ILogger<TeamRepository> logger, int commandTimeout = 0)
        : base(config, logger, commandTimeout)
    {
    }

    /// <inheritdoc />
    public async Task<ResultOf<int>> GetUserTeamsIdAsync(UserQuery userQuery)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterNVarchar("@UserId", userQuery.UserId),
            GetSqlParameterUniqueIdentifier("@TeamId", userQuery.TeamId)
        };

        ResultOf<int> result = await ExecuteStoredProcedureAsync("sp_GetUserTeamId", parameters);

        if (result.HasError)
        {
            _logger.LogError("Erreur lors de la récupération des équipes de l'utilisateur {UserId} : {Error}", userQuery.UserId, result.Error);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultOf<List<EquipeDb>>> GetTeamsByUserIdAsync(string userId)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterNVarchar("@UserId", userId)
        };

        ResultOf<List<EquipeDb>> result = await ExecuteStoredProcedureWithJsonResultAsync<List<EquipeDb>>("sp_GetTeamsByUser_Light", parameters);
        
        if (result.HasError)
        {
            _logger.LogError("Erreur lors de la récupération des équipes de l'utilisateur {UserId} : {Error}", userId, result.Error);
        }

        return result;
    }
}
