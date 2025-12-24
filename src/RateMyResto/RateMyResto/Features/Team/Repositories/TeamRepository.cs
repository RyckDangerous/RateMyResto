using Microsoft.Data.SqlClient;
using RateMyResto.Core.Data;
using RateMyResto.Features.Shared.Configurations;
using RateMyResto.Features.Team.Models;

namespace RateMyResto.Features.Team.Repositories;

public sealed class TeamRepository : RepositoryBase<TeamRepository>, ITeamRepository
{

    public TeamRepository(IApplicationSettings settings, ILogger<TeamRepository> logger)
        : base(settings, logger)
    {
    }

    /// <inheritdoc />
    public async Task<ResultOf> CreateTeamAsync(TeamCommand command)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterUniqueIdentifier("@IdTeam", command.IdTeam),
            GetSqlParameterNVarchar("@Nom", command.Nom),
            GetSqlParameterNVarchar("@Description", command.Description),
            GetSqlParameterNVarchar("@Owner", command.Owner.ToString())
        };

        ResultOf teamResult = await ExecuteStoredProcedureAsync("sp_CreateTeam", parameters);

        if (teamResult.HasError)
        {
            _logger.LogError("Erreur sur la création de l'équipe : {Error}", teamResult.Error);
        }

        // Ajouter l'utilisateur en tant que membre de l'équipe
        SqlParameter[] memberParameters =
        {
            GetSqlParameterNVarchar("@UserId", command.Owner.ToString()),
            GetSqlParameterUniqueIdentifier("@TeamId", command.IdTeam)
        };

        ResultOf addMemberResult = await ExecuteNonQueryStoredProcedureAsync("sp_AddMemberToTeam", memberParameters);

        if (addMemberResult.HasError)
        {
            string errorMessage = "Erreur lors de l'ajout du membre à l'équipe";
            _logger.LogError(errorMessage, addMemberResult.Error);

            SqlServerError sqlError = new(errorMessage, addMemberResult.Error);

            return ResultOf.Failure(sqlError);
        }

        return ResultOf.Success();
    }

    /// <inheritdoc />
    public async Task<ResultOf<List<TeamDb>>> GetTeamByOwnerAsync(string userId)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterNVarchar("@Owner", userId)
        };

        return await ExecuteStoredProcedureWithJsonResult<List<TeamDb>>("sp_GetTeamByOwner", parameters);
    }

    /// <inheritdoc />
    public async Task<ResultOf<List<TeamDb>>> GetTeamsByMemberAsync(string userId)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterNVarchar("@UserId", userId)
        };

        return await ExecuteStoredProcedureWithJsonResult<List<TeamDb>>("sp_GetTeamsByUser", parameters);
    }


    public async Task<ResultOf> JoinTeamAsync(Guid teamId, string userId)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterNVarchar("@UserId", userId),
            GetSqlParameterUniqueIdentifier("@TeamId", teamId)
        };

        return await ExecuteNonQueryStoredProcedureAsync("sp_AddMemberToTeam", parameters);
    }

    /// <inheritdoc />
    public async Task<ResultOf> DeleteTeamMemberAsync(Guid idTeam, string userId)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterNVarchar("@UserId", userId),
            GetSqlParameterUniqueIdentifier("@IdTeam", idTeam)
        };

        return await ExecuteNonQueryStoredProcedureAsync("sp_RemoveUserFromTeam", parameters);
    }

}
