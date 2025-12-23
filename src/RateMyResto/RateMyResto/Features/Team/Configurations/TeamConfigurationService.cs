
using RateMyResto.Features.Team.Repositories;
using RateMyResto.Features.Team.Services;

namespace RateMyResto.Features.Team.Configurations;

public static class TeamConfigurationService
{
    /// <summary>
    /// Ajout des services de migration de la base de données
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTeamFeatures(this IServiceCollection services)
    {
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ITeamViewService, TeamViewService>();

        return services;
    }

}
