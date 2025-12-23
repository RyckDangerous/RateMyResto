
using RateMyResto.Core.Models.Errors;

namespace RateMyResto.Features.DbMigration.Errors;

public sealed class DbUpError : Error
{
    public DbUpError(string message)
        : base(message)
    {
    }

    public DbUpError(string message, string urlDoc)
        : base(message, urlDoc)
    {
    }

    public DbUpError(string message, Exception exception)
        : base(message, exception)
    {
    }

    public DbUpError(string message, Error innerError)
        : base(message, innerError)
    {
    }

    public DbUpError(string message, string urlDoc, Exception exception)
        : base(message, urlDoc, exception)
    {
    }

    public DbUpError(string message, string urlDoc, Error innerError)
        : base(message, urlDoc, innerError)
    {
    }
}
