
namespace RateMyResto.Features.EventDetail.Models.Errors;

public sealed class FileError : Error
{
    /// <summary>
    /// Constructeur de l'erreur de fichier avec message.
    /// </summary>
    /// <param name="message"></param>
    public FileError(string message) 
        : base(message)
    {
    }

    /// <summary>
    /// Constructeur de l'erreur de fichier avec message et URL de documentation.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="urlDoc"></param>
    public FileError(string message, string urlDoc) 
        : base(message, urlDoc)
    {
    }

    /// <summary>
    /// Constructeur de l'erreur de fichier avec message et exception.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public FileError(string message, Exception exception) 
        : base(message, exception)
    {
    }

    /// <summary>
    /// Constructeur de l'erreur de fichier avec message et erreur interne.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerError"></param>
    public FileError(string message, Error innerError) 
        : base(message, innerError)
    {
    }

    /// <summary>
    /// Constructeur de l'erreur de fichier avec message, URL de documentation et exception.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="urlDoc"></param>
    /// <param name="exception"></param>
    public FileError(string message, string urlDoc, Exception exception) 
        : base(message, urlDoc, exception)
    {
    }

    /// <summary>
    /// Constructeur de l'erreur de fichier avec message, URL de documentation et erreur interne.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="urlDoc"></param>
    /// <param name="innerError"></param>
    public FileError(string message, string urlDoc, Error innerError) 
        : base(message, urlDoc, innerError)
    {
    }
}
