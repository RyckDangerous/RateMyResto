namespace RateMyResto.Core.Configurations;

public static class ApplicationSettingsConfiguration
{
    /// <summary>
    /// Retourne la configuration de l'application.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static string ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        IApplicationSettings configSettings = BindConfiguration(configuration);
        services.AddSingleton(configSettings);

        return configSettings.GetSqlServerConnection();
    }

    /// <summary>
    /// Lie la configuration de l'application à partir du fichier appsettings.json.
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    private static IApplicationSettings BindConfiguration(IConfiguration configuration)
    {
        string? server = configuration.GetConnectionString("Server");
        ArgumentNullException.ThrowIfNullOrEmpty(server);

        string? dbName = configuration.GetConnectionString("Database");
        ArgumentNullException.ThrowIfNullOrEmpty(dbName);

        string? login = configuration.GetConnectionString("Login");
        ArgumentNullException.ThrowIfNullOrEmpty(login);

        string? password = configuration.GetConnectionString("Password");
        ArgumentNullException.ThrowIfNullOrEmpty(password);

        IApplicationSettings config = new ApplicationSettings()
        {
            Dbname = dbName,
            SqlServer = server,
            UserLogin = login,
            UserPassword = password
        };

        return config;
    }
}
