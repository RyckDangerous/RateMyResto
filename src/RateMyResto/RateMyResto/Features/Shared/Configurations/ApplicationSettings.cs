using Microsoft.Data.SqlClient;

namespace RateMyResto.Features.Shared.Configurations;

public struct ApplicationSettings : IApplicationSettings
{
    /// <inheritdoc/>
    public required string Dbname { get; init; }

    /// <inheritdoc/>
    public required string SqlServer { get; init; }

    /// <inheritdoc/>
    public required string UserLogin { get; init; }

    /// <inheritdoc/>
    public required string UserPassword { get; init; }

    /// <inheritdoc/>
    public required string AdminPassword { get; init; }

    /// <inheritdoc/>
    public string GetSqlServerConnection()
    {
        SqlConnectionStringBuilder builder = new ()
        {
            DataSource = $"{SqlServer}",
            InitialCatalog = Dbname,
            UserID = UserLogin,
            Password = UserPassword,
            TrustServerCertificate = true,
            MultipleActiveResultSets = true,
            Encrypt = true,
            ConnectTimeout = 30
        };


        return builder.ConnectionString;
    }

}
