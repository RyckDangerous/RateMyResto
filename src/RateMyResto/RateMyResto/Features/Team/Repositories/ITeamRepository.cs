using RateMyResto.Features.Team.Models;

namespace RateMyResto.Features.Team.Repositories;

public interface ITeamRepository
{
    /// <summary>
    /// Crée une nouvelle équipe
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<ResultOf> CreateTeamAsync(TeamCommand command);

    /// <summary>
    /// Récupère les équipes d'un utilisateur
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ResultOf<List<TeamDb>>> GetTeamByOwnerAsync(string userId);

    /// <summary>
    /// Récupère les équipes dont un utilisateur est membre
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ResultOf<List<TeamDb>>> GetTeamsByMemberAsync(string userId);

    /// <summary>
    /// Supprime un membre d'une équipe
    /// </summary>
    /// <param name="idTeam"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ResultOf> DeleteTeamMemberAsync(Guid idTeam, string userId);

    /// <summary>
    /// Permet à un utilisateur de rejoindre une équipe
    /// </summary>
    /// <param name="teamId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ResultOf> JoinTeamAsync(Guid teamId, string userId);
}