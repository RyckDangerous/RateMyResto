using RateMyResto.Features.Shared.Components.SnackbarComponent;
using RateMyResto.Features.Shared.Components.DrawerComponent;

namespace RateMyResto.Features.Shared.Configurations;

public static class SharedConfigurationService
{
    /// <summary>
    /// Enregistre les services partagés
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<ISnackbarService, SnackbarService>();
        services.AddScoped<IDrawerService, DrawerService>();

        return services;
    }
}

