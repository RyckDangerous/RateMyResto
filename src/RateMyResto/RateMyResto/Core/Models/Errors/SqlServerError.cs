
namespace RateMyResto.Core.Models.Errors;

public sealed class SqlServerError : Error
{
    public SqlServerError(string message) 
        : base(message)
    {
    }

    public SqlServerError(string message, string urlDoc) 
        : base(message, urlDoc)
    {
    }

    public SqlServerError(string message, Exception exception) 
        : base(message, exception)
    {
    }

    public SqlServerError(string message, Error innerError) 
        : base(message, innerError)
    {
    }

    public SqlServerError(string message, string urlDoc, Exception exception) 
        : base(message, urlDoc, exception)
    {
    }

    public SqlServerError(string message, string urlDoc, Error innerError) 
        : base(message, urlDoc, innerError)
    {
    }
}
