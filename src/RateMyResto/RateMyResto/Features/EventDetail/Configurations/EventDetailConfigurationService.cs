using Microsoft.Extensions.DependencyInjection;
using RateMyResto.Features.EventDetail.Repositories;
using RateMyResto.Features.EventDetail.Services;

namespace RateMyResto.Features.EventDetail.Configurations;

public static class EventDetailConfigurationService
{
    public static IServiceCollection AddEventDetailFeatures(this IServiceCollection services)
    {
        // Enregistrement des repositories
        services.AddScoped<IEventDetailRepository, EventDetailRepository>();

        // Enregistrement des services
        services.AddScoped<IEventDetailViewService, EventDetailViewService>();

        return services;
    }
}

