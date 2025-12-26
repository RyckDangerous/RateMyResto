using RateMyResto.Features.Event.Models.Dbs;
using RateMyResto.Features.Event.Models.Queries;

namespace RateMyResto.Features.Event.Repositories;

public interface ITeamRepository
{
    /// <summary>
    /// Récupère l'identifiant de l'utilisateur dans l'équipe.
    /// </summary>
    /// <param name="userQuery"></param>
    /// <returns></returns>
    Task<ResultOf<int>> GetUserTeamsIdAsync(UserQuery userQuery);

    /// <summary>
    /// Récupère les équipes associées à un utilisateur donné.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ResultOf<List<EquipeDb>>> GetTeamsByUserIdAsync(string userId);
}