using Microsoft.Data.SqlClient;
using RateMyResto.Core.Data;
using RateMyResto.Features.Event.Models.Commands;
using RateMyResto.Features.Shared.Configurations;

namespace RateMyResto.Features.Event.Repositories;

public sealed class RestaurantRepository : RepositoryBase<RestaurantRepository>, IRestaurantRepository
{
    public RestaurantRepository(IApplicationSettings config, ILogger<RestaurantRepository> logger, int commandTimeout = 0)
        : base(config, logger, commandTimeout)
    {
    }

    /// <inheritdoc />
    public async Task<ResultOf<bool>> IsRestaurantExistAsync(string nom)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterNVarchar("@Nom", nom)
        };

        ResultOf<int> result = await ExecuteStoredProcedureAsync("sp_CheckRestaurantExistByName", parameters);

        if (result.HasError)
        {
            _logger.LogError("Erreur lors de la vérification de l'existence du restaurant : {Error}", result.Error);
            return ResultOf.Failure<bool>(result.Error);
        }

        if(result.Value == 0)
        {
            return ResultOf.Success(false);
        }

        return ResultOf.Success(true);
    }

    /// <inheritdoc />
    public async Task<ResultOf<int>> CreateRestaurantAsync(CreateRestaurantCommand command)
    {
        SqlParameter[] parameters =
        {
            GetSqlParameterNVarchar("@Nom", command.Nom),
            GetSqlParameterNVarchar("@Adresse", command.Adresse),
            GetSqlParameterNVarchar("@LienGoogleMaps", command.UrlGoogleMaps)
        };

        ResultOf<int> teamResult = await ExecuteStoredProcedureAsync("sp_CreateRestaurant", parameters);

        if (teamResult.HasError)
        {
            _logger.LogError("Erreur sur la création du restaurant : {Error}", teamResult.Error);
        }

        return teamResult;
    }
}
