using RateMyResto.Core.Models;
using RateMyResto.Features.Account.ManageAccountFeature.Models;

namespace RateMyResto.Features.Account.ManageAccountFeature.Repositories;

/// <summary>
/// Repository pour la gestion des comptes utilisateurs.
/// </summary>
public interface IManageAccountRepository
{
    /// <summary>
    /// Récupère tous les utilisateurs de la base, à l'exception du compte admin.
    /// </summary>
    /// <returns>Liste des utilisateurs</returns>
    Task<ResultOf<List<UserItemViewModel>>> GetUsersExceptAdminAsync();

    /// <summary>
    /// Réinitialise le mot de passe d'un utilisateur.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur</param>
    /// <param name="newPassword">Nouveau mot de passe</param>
    /// <returns>Résultat de l'opération</returns>
    Task<ResultOf> ResetPasswordAsync(string userId, string newPassword);
}
