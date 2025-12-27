using RateMyResto.Features.Shared.Services;
using RateMyResto.Features.Team.Models;

namespace RateMyResto.Features.Team.Services;

public interface ITeamViewService : IViewServiceBase
{
    /// <summary>
    /// État de la modale de création
    /// </summary>
    bool ShowCreateModal { get; set; }

    /// <summary>
    /// Nom de la nouvelle équipe
    /// </summary>
    string NewTeamName { get; set; }

    /// <summary>
    /// Description de la nouvelle équipe
    /// </summary>
    string? NewTeamDescription { get; set; }

    /// <summary>
    /// État de la modale pour rejoindre une équipe
    /// </summary>
    bool ShowJoinModal { get; }

    /// <summary>
    /// Code de l'équipe à rejoindre
    /// </summary>
    string TeamCodeToJoin { get; set; }

    /// <summary>
    /// Message d'erreur lors de la jonction à une équipe
    /// </summary>
    string? JoinErrorMessage { get; }

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
    /// Gère la création d'une équipe
    /// </summary>
    /// <returns></returns>
    Task HandleCreateTeam();

    /// <summary>
    /// Retire un membre d'une équipe
    /// </summary>
    /// <param name="teamId">ID de l'équipe</param>
    /// <param name="userId">ID de l'utilisateur à retirer</param>
    /// <returns></returns>
    Task RemoveMemberAsync(Guid teamId, string userId);

    /// <summary>
    /// Ouvre la modale de création d'équipe
    /// </summary>
    void OpenCreateModal();

    /// <summary>
    /// Ferme la modale de rejoindre une équipe
    /// </summary>
    void CloseCreateModal();

    /// <summary>
    /// Ouvre le drawer d'une équipe
    /// </summary>
    /// <param name="teamId"></param>
    void OpenTeamDrawer(Guid teamId);

    /// <summary>
    /// Ouvre le drawer d'une équipe en mode membre (lecture seule)
    /// </summary>
    /// <param name="teamId"></param>
    void OpenMemberTeamDrawer(Guid teamId);

    /// <summary>
    /// Ouvre la modale de rejoindre une équipe
    /// </summary>
    void OpenJoinModal();

    /// <summary>
    /// Ferme la modale de rejoindre une équipe
    /// </summary>
    void CloseJoinModal();

    /// <summary>
    /// Gère la jonction à une équipe via un code
    /// </summary>
    /// <returns></returns>
    Task HandleJoinTeam();

    /// <summary>
    /// Gère le départ d'une équipe
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    Task HandleLeaveTeam(Guid teamId);
}
