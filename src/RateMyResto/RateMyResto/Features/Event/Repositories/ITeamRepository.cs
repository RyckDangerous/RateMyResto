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
}