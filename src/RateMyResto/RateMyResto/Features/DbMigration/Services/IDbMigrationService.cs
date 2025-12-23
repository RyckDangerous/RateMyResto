namespace RateMyResto.Features.DbMigration.Services;

public interface IDbMigrationService
{
    /// <summary>
    /// Effectue la migration de la base de données
    /// </summary>
    /// <returns></returns>
    bool UpgradeDatabase();
}
