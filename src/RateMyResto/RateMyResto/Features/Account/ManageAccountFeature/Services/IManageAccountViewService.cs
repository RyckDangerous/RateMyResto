using RateMyResto.Features.Account.ManageAccountFeature.Models;
using RateMyResto.Features.Shared.Services;

namespace RateMyResto.Features.Account.ManageAccountFeature.Services;

/// <summary>
/// Interface du ViewService pour la page de gestion des comptes.
/// </summary>
public interface IManageAccountViewService : IViewServiceBase
{
    /// <summary>
    /// ViewModel de la page de gestion des comptes.
    /// </summary>
    ManageAccountViewModel ViewModel { get; }

    /// <summary>
    /// Indique si la modale de reset de mot de passe est affichée.
    /// </summary>
    bool ShowResetPasswordModal { get; }

    /// <summary>
    /// ID de l'utilisateur sélectionné pour le reset de mot de passe.
    /// </summary>
    string? SelectedUserId { get; }

    /// <summary>
    /// Nom de l'utilisateur sélectionné pour le reset de mot de passe.
    /// </summary>
    string? SelectedUserName { get; }

    /// <summary>
    /// Nouveau mot de passe saisi dans la modale.
    /// </summary>
    string NewPassword { get; set; }

    /// <summary>
    /// Indique si le nouveau mot de passe respecte toutes les règles.
    /// </summary>
    bool IsNewPasswordValid { get; }

    /// <summary>
    /// Charge la liste des utilisateurs.
    /// </summary>
    Task LoadUsersAsync();

    /// <summary>
    /// Ouvre la modale de reset de mot de passe pour un utilisateur.
    /// </summary>
    /// <param name="userId">ID de l'utilisateur</param>
    /// <param name="userName">Nom de l'utilisateur</param>
    void OpenResetPasswordModal(string userId, string userName);

    /// <summary>
    /// Ferme la modale de reset de mot de passe.
    /// </summary>
    void CloseResetPasswordModal();

    /// <summary>
    /// Réinitialise le mot de passe de l'utilisateur sélectionné et ferme la modale.
    /// </summary>
    Task HandleResetPasswordAsync();
}
