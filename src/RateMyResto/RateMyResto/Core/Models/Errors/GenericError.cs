namespace RateMyResto.Core.Models.Errors;

/// <summary>
/// Erreur générique pour les erreurs non spécifiques.
/// </summary>
public sealed class GenericError : Error
{
    /// <summary>
    /// Initialise une nouvelle instance avec un message.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    public GenericError(string message) 
        : base(message)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance avec un message et une URL de documentation.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="urlDoc">URL vers une documentation ou aide supplémentaire.</param>
    public GenericError(string message, string urlDoc) 
        : base(message, urlDoc)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance avec un message et l'exception d'origine.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="exception">Exception ayant conduit à cette erreur.</param>
    public GenericError(string message, Exception exception) 
        : base(message, exception)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance avec un message et une erreur interne.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="innerError">Erreur interne associée pour plus de détail.</param>
    public GenericError(string message, Error innerError) 
        : base(message, innerError)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance avec un message, une URL de documentation et une exception.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="urlDoc">URL vers une documentation ou aide supplémentaire.</param>
    /// <param name="exception">Exception ayant conduit à cette erreur.</param>
    public GenericError(string message, string urlDoc, Exception exception) 
        : base(message, urlDoc, exception)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance avec un message, une URL de documentation et une erreur interne.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="urlDoc">URL vers une documentation ou aide supplémentaire.</param>
    /// <param name="innerError">Erreur interne associée pour plus de détail.</param>
    public GenericError(string message, string urlDoc, Error innerError) 
        : base(message, urlDoc, innerError)
    {
    }
}

