namespace RateMyResto.Features.Shared.Configurations;

public sealed class ApplicationSettings : IApplicationSettings
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
    public string GetSqlServerConnection()
    {
        return $"Server={SqlServer};Database={Dbname};User Id={UserLogin};Password={UserPassword};TrustServerCertificate=True;MultipleActiveResultSets=true;";
    }

}
