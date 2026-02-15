using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RateMyResto.Core.Models;
using RateMyResto.Core.Models.Errors;
using RateMyResto.Features.Account.ManageAccountFeature.Models;
using RateMyResto.Features.Account.Shared.Constantes;
using RateMyResto.Features.Data;

namespace RateMyResto.Features.Account.ManageAccountFeature.Repositories;

/// <summary>
/// Repository pour la gestion des comptes utilisateurs.
/// Utilise Identity (UserManager et ApplicationDbContext).
/// </summary>
public sealed class ManageAccountRepository : IManageAccountRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ManageAccountRepository> _logger;

    public ManageAccountRepository(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext,
        ILogger<ManageAccountRepository> logger)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ResultOf<List<UserItemViewModel>>> GetUsersExceptAdminAsync()
    {
        try
        {
            List<UserItemViewModel> users = await _dbContext.Users
                .Where(u => u.UserName != UserConstantes.AdminUserName)
                .OrderBy(u => u.UserName)
                .Select(u => new UserItemViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName ?? string.Empty
                })
                .ToListAsync();

            return ResultOf.Success(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs");
            return ResultOf.Failure<List<UserItemViewModel>>(
                new GenericError("Erreur lors de la récupération des utilisateurs", ex));
        }
    }

    /// <inheritdoc />
    public async Task<ResultOf> ResetPasswordAsync(string userId, string newPassword)
    {
        try
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                _logger.LogWarning("Utilisateur non trouvé pour le reset de mot de passe : {UserId}", userId);
                return ResultOf.Failure(
                    new GenericError("Utilisateur non trouvé."));
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            IdentityResult result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Erreur lors du reset du mot de passe : {Errors}", errors);
                return ResultOf.Failure(
                    new GenericError($"Impossible de réinitialiser le mot de passe : {errors}"));
            }

            return ResultOf.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du reset du mot de passe pour l'utilisateur {UserId}", userId);
            return ResultOf.Failure(
                new GenericError("Erreur lors de la réinitialisation du mot de passe.", ex));
        }
    }
}
