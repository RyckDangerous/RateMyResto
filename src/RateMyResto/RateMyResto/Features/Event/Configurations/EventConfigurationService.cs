using RateMyResto.Features.Event.Repositories;
using RateMyResto.Features.Event.Services;

namespace RateMyResto.Features.Event.Configurations;

public static class EventConfigurationService
{
    /// <summary>
    /// Ajout des services Event
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddEventFeatures(this IServiceCollection services)
    {
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<Repositories.ITeamRepository, Repositories.TeamRepository>();
        services.AddScoped<EventViewService>();

        return services;
    }
}
