using RateMyResto.Core.Data;
using RateMyResto.Features.HomeFeature.Home.Models;
using RateMyResto.Features.Shared.Configurations;

namespace RateMyResto.Features.HomeFeature.Home.Repositories;

/// <summary>
/// Implémentation du repository pour la page d'accueil publique.
/// </summary>
public sealed class HomeRepository : RepositoryBase<HomeRepository>, IHomeRepository
{
    public HomeRepository(IApplicationSettings config, ILogger<HomeRepository> logger)
        : base(config, logger)
    {
    }

    /// <inheritdoc />
    public async Task<ResultOf<List<TeamPublicDb>>> GetAllTeamsAsync()
    {
        ResultOf<List<TeamPublicDb>> result =
            await ExecuteStoredProcedureWithJsonResultAsync<List<TeamPublicDb>>(
                procName: "sp_GetAllTeamsPublic");

        if (result.HasError && result.Error is NotFoundError)
        {
            return ResultOf.Success(new List<TeamPublicDb>());
        }

        return result;
    }
}
