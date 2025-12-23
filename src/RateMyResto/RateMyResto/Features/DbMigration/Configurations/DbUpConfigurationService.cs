using RateMyResto.Features.DbMigration.Services;

namespace RateMyResto.Features.DbMigration.Configurations;

public static class DbUpConfigurationService
{
    /// <summary>
    /// Ajout des services de migration de la base de données
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDbMigrationServices(this IServiceCollection services)
    {
        services.AddSingleton<IDbMigrationService, DbMigrationService>();

        return services;
    }

}
