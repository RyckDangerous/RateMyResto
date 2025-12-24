using RateMyResto.Features.Team.Models;

namespace RateMyResto.Features.Team.Services;

public interface ITeamViewService
{
    /// <summary>
    /// Modèle de vue de l'équipe
    /// </summary>
    TeamViewModel ViewModel { get; }

    /// <summary>
    /// Charge le ViewModel
    /// </summary>
    /// <returns></returns>
    Task LoadViewModelAsync();

    /// <summary>
    /// Rejoint une équipe
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    Task JoinTeamAsync(Guid teamId);

    /// <summary>
    /// Quitte une équipe
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    Task LeaveTeamAsync(Guid teamId);
}
