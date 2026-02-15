
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RateMyResto.Features.Account.Shared.Constantes;
using RateMyResto.Features.Data;
using RateMyResto.Features.Shared.Configurations;

namespace RateMyResto.Features.Account.Services;

public sealed class CreateAdminService : ICreateAdminService
{
    private readonly IApplicationSettings _applicationSettings;
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CreateAdminService> _logger;

    public CreateAdminService(IApplicationSettings applicationSettings,
                            ApplicationDbContext dbContext,
                            IUserStore<ApplicationUser> userStore,
                            UserManager<ApplicationUser> userManager,
                            ILogger<CreateAdminService> logger)
    {
        _applicationSettings = applicationSettings;
        _dbContext = dbContext;
        _userStore = userStore;
        _userManager = userManager;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task CreateAdminAsync()
    {
        try
        {
            ApplicationUser userAdmin = new ApplicationUser
            {
                UserName = UserConstantes.AdminUserName,
                Email = "admin@example.com"
            };
            await _userStore.SetUserNameAsync(userAdmin, UserConstantes.AdminUserName, CancellationToken.None);

            IUserEmailStore<ApplicationUser>? emailStore = (IUserEmailStore<ApplicationUser>)_userStore;
            await emailStore.SetEmailAsync(userAdmin, userAdmin.Email, CancellationToken.None);

            IdentityResult result = await _userManager.CreateAsync(userAdmin, _applicationSettings.AdminPassword);
            if (!result.Succeeded)
            {
                _logger.LogError("Erreur lors de la création du compte administrateur: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return;
            }

            _logger.LogInformation("Compte Admin créé.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du compte administrateur");
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsAdminExist()
    {
        try
        {
            return await _dbContext.Users.AnyAsync(u => u.UserName == UserConstantes.AdminUserName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification de l'existence du compte administrateur");
        }

        return false;
    }
}
