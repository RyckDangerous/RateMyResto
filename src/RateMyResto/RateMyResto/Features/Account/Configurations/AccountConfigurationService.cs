using RateMyResto.Features.Account.Services;

namespace RateMyResto.Features.Account.Configurations;

public static class AccountConfigurationService
{
    /// <summary>
    /// Enregistre les services liés à la gestion des comptes, notamment la création du compte administrateur.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAccountFeatures(this IServiceCollection services)
    {
        services.AddTransient<ICreateAdminService, CreateAdminService>();

        return services;
    }

}
