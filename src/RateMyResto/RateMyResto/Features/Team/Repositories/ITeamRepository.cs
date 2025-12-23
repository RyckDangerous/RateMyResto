using RateMyResto.Features.Team.Models;

namespace RateMyResto.Features.Team.Repositories;

public interface ITeamRepository
{
    /// <summary>
    /// Crée une nouvelle équipe
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<ResultOf> CreateTeam(TeamCommand command);

    /// <summary>
    /// Récupère les équipes d'un utilisateur
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ResultOf<List<TeamDb>>> GetTeamByOwner(string userId);

    /// <summary>
    /// Récupère les équipes dont un utilisateur est membre
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ResultOf<List<TeamDb>>> GetTeamsByMember(string userId);
}