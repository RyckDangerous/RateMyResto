using RateMyResto.Features.HomeFeature.Home.Repositories;
using RateMyResto.Features.HomeFeature.Home.ViewServices;
using RateMyResto.Features.HomeFeature.RestoDetail.Repositories;
using RateMyResto.Features.HomeFeature.RestoDetail.ViewServices;
using RateMyResto.Features.HomeFeature.TeamsResto.Repositories;
using RateMyResto.Features.HomeFeature.TeamsResto.ViewServices;

namespace RateMyResto.Features.HomeFeature.Configurations;

/// <summary>
/// Enregistrement des dépendances de la feature HomeFeature (vitrine publique).
/// </summary>
public static class HomeFeatureConfiguration
{
    /// <summary>
    /// Ajoute les services de la feature HomeFeature au conteneur DI :
    /// repositories et view services pour la page d'accueil, les sorties d'équipe
    /// et le détail d'un événement.
    /// </summary>
    /// <param name="services">Collection de services.</param>
    public static IServiceCollection AddHomeFeature(this IServiceCollection services)
    {
        services.AddScoped<IHomeRepository, HomeRepository>();
        services.AddScoped<IHomeViewService, HomeViewService>();
        services.AddScoped<ITeamsRestoRepository, TeamsRestoRepository>();
        services.AddScoped<ITeamsRestoViewService, TeamsRestoViewService>();
        services.AddScoped<IRestoDetailRepository, RestoDetailRepository>();
        services.AddScoped<IRestoDetailViewService, RestoDetailViewService>();

        return services;
    }
}
