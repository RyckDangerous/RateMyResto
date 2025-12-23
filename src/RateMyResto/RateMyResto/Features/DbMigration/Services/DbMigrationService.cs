using DbUp;
using DbUp.Engine;
using RateMyResto.Features.DbMigration.Errors;
using RateMyResto.Features.Shared.Configurations;
using System.Reflection;

namespace RateMyResto.Features.DbMigration.Services;

public sealed class DbMigrationService : IDbMigrationService
{
    private readonly IApplicationSettings _config;
    private readonly ILogger<DbMigrationService> _logger;

    public DbMigrationService(IApplicationSettings config, ILogger<DbMigrationService> logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Mise à jour de la base de données SqlServer via scripts
    /// </summary>
    /// <returns></returns>
    public bool UpgradeDatabase()
    {
        _logger.LogInformation("DbMigrationService Started");

        try
        {
            string connexionString = _config.GetSqlServerConnection();

            UpgradeEngine upgrader = DeployChanges.To.SqlDatabase(connexionString, "dbo")
                                              .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                                              .LogTo(_logger)
                                              .Build();

            if (!upgrader.IsUpgradeRequired())
            {
                _logger.LogInformation("Aucune migration requise.");
                return true;
            }

            _logger.LogInformation("Migration de la Base en cours ...");
            DatabaseUpgradeResult result = upgrader.PerformUpgrade();

            if (result.Successful)
            {
                _logger.LogInformation("Migration de la Base terminée avec Succés");
                return true;
            }
            else
            {
                DbUpError dbUpError = new("Erreur sur Upgrade de DbUp", result.Error);
                _logger.LogError(dbUpError);

                return false;
            }
        }
        catch (Exception ex)
        {
            DbUpError dbUpError = new("Erreur lors de la migration de la base de données", ex);
            _logger.LogError(dbUpError);

            return false;
        }
    }
}