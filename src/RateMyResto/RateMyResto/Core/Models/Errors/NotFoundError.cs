namespace RateMyResto.Core.Models.Errors;

/// <summary>
/// Erreur indiquant qu'une ressource demandée est introuvable (HTTP 404 logique).
/// Peut embarquer une exception et/ou une erreur interne pour le diagnostic.
/// </summary>
public sealed class NotFoundError : Error
{
    /// <summary>
    /// Initialise une nouvelle instance avec un message décrivant la ressource introuvable.
    /// </summary>
    /// <param name="message">Message décrivant le contexte de l'élément non trouvé.</param>
    public NotFoundError(string message) 
        : base(message)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance avec un message et une URL de documentation.
    /// </summary>
    /// <param name="message">Message décrivant le contexte de l'élément non trouvé.</param>
    /// <param name="urlDoc">URL vers une documentation ou aide supplémentaire.</param>
    public NotFoundError(string message, string urlDoc) 
        : base(message, urlDoc)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance avec un message et l'exception d'origine.
    /// </summary>
    /// <param name="message">Message décrivant le contexte de l'élément non trouvé.</param>
    /// <param name="exception">Exception ayant conduit à cette erreur.</param>
    public NotFoundError(string message, Exception exception) 
        : base(message, exception)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance avec un message et une erreur interne.
    /// </summary>
    /// <param name="message">Message décrivant le contexte de l'élément non trouvé.</param>
    /// <param name="innerError">Erreur interne associée pour plus de détail.</param>
    public NotFoundError(string message, Error innerError) 
        : base(message, innerError)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance avec un message, une URL de documentation et une exception.
    /// </summary>
    /// <param name="message">Message décrivant le contexte de l'élément non trouvé.</param>
    /// <param name="urlDoc">URL vers une documentation ou aide supplémentaire.</param>
    /// <param name="exception">Exception ayant conduit à cette erreur.</param>
    public NotFoundError(string message, string urlDoc, Exception exception) 
        : base(message, urlDoc, exception)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance avec un message, une URL de documentation et une erreur interne.
    /// </summary>
    /// <param name="message">Message décrivant le contexte de l'élément non trouvé.</param>
    /// <param name="urlDoc">URL vers une documentation ou aide supplémentaire.</param>
    /// <param name="innerError">Erreur interne associée pour plus de détail.</param>
    public NotFoundError(string message, string urlDoc, Error innerError) 
        : base(message, urlDoc, innerError)
    {
    }
}
