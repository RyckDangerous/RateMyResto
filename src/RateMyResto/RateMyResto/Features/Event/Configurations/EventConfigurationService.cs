namespace RateMyResto.Features.Event.Configurations;

public static class EventConfigurationService
{
    /// <summary>
    /// Ajout des services de migration de la base de données
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddEventFeatures(this IServiceCollection services)
    {
        // services.AddScoped<IEventRepository, EventRepository>();
        // services.AddScoped<IEventService, EventService>();

        return services;
    }
}
