
using RateMyResto.Core.Models.Errors;

namespace RateMyResto.Core.Logging;

public static class ErrorLog
{
    /// <summary>
    /// Log une "Error" en tant qu'erreur
    /// via le "nlogsettings.json", le message sera loggé en type Technical
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="error"></param>
    /// <param name="args"></param>
    public static void LogError(this ILogger logger, Error error, params object?[] args)
    {
        logger.LogWithError(error, LogLevel.Error, error.Message, args);
    }

    /// <summary>
    /// Log une "Error" en Warning
    /// via le "nlogsettings.json", le message sera loggé en type Technical
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="error"></param>
    /// <param name="args"></param>
    public static void LogWarning(this ILogger logger, Error error, params object?[] args)
    {
        logger.LogWithError(error, LogLevel.Warning, error.Message, args);
    }

    /// <summary>
    /// Log une "Error" en Information
    /// via le "nlogsettings.json", le message sera loggé en type Technical
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="error"></param>
    /// <param name="args"></param>
    public static void LogInformation(this ILogger logger, Error error, params object?[] args)
    {
        logger.LogWithError(error, LogLevel.Information, error.Message, args);
    }

    /// <summary>
    /// Log une "Error"
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="error"></param>
    /// <param name="logLevel"></param>
    /// <param name="eventId"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    private static void LogWithError(this ILogger logger, Error error, LogLevel logLevel, string? message, params object?[] args)
    {
        Dictionary<string, object> errorData = new()
        {
            ["error_data"] = GetErrorData(error)
        };

        using (logger.BeginScope(errorData))
        {
            if (error.Exception is null)
            {
                logger.Log(logLevel, message ?? error.Message, args);
            }
            else
            {
                logger.Log(logLevel, error.Exception, message ?? error.Message, args);
            }
        }
    }

    /// <summary>
    /// Extrait les données "Data" de l'erreur pour les logger.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    private static Dictionary<string, object> GetErrorData(Error error)
    {
        return new Dictionary<string, object>(error.Data.Count + 1)
        {
            ["error_type"] = error.GetType().Name,
            ["error_url_doc"] = error.UrlDoc ?? "NoDoc",
            ["data"] = error.Data.Count > 0
                    ? error.Data.Select(kvp => new
                    {
                        key = kvp.Key,
                        value = kvp.Value
                    }).ToList()
                    : new List<object>()
        };
    }
}