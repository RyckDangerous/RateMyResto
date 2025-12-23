namespace RateMyResto.Features.Shared.Configurations;

public interface IApplicationSettings
{
    /// <summary>
    /// Nom de la base de donnée
    /// </summary>
    string Dbname { get; }

    /// <summary>
    /// Chaîne de connexion pour la base de données
    /// </summary>
    string SqlServer { get; }

    /// <summary>
    /// Login SQL pour la base de données
    /// </summary>
    string UserLogin { get; }

    /// <summary>
    /// Mot de passe SQL pour la base de données
    /// </summary>
    string UserPassword { get; }

    /// <summary>
    /// Obtient la chaîne de connexion pour SQL Server.
    /// </summary>
    /// <returns></returns>
    string GetSqlServerConnection();

}
