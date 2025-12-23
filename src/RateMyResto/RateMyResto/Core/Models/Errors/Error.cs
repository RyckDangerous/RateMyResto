using System.Text.Json;

namespace RateMyResto.Core.Models.Errors;

public abstract class Error
{
    /// <summary>
    /// Pour avoir un message particulier
    /// </summary>        
    public string Message { get; set; }

    /// <summary>
    /// URL pour une documentation de l'erreur.
    /// </summary>
    public string? UrlDoc { get; set; }

    /// <summary>
    /// Si l'erreur vient d'une Exception
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Peut contenir une erreur interne.
    /// </summary>
    public Error? InnerError { get; set; }

    /// <summary>
    /// Données qui peuvent être utiles pour l'erreur.
    /// exemple pour du Log.
    /// </summary>
    public Dictionary<string, object> Data { get; private set; } = [];

    /// <summary>
    /// Constructeur de base pour l'erreur.
    /// </summary>
    /// <param name="message"></param>
    protected Error(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Constructeur de base pour l'erreur avec documentation.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="urlDoc"></param>
    protected Error(string message, string urlDoc)
        : this(message)
    {
        UrlDoc = urlDoc;
    }

    /// <summary>
    /// Constructeur de base pour l'erreur avec une exception.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    protected Error(string message, Exception exception)
        : this(message)
    {
        Exception = exception;
    }

    /// <summary>
    /// Constructeur de base pour l'erreur avec documentation et l'exception.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="urlDoc"></param>
    /// <param name="exception"></param>
    protected Error(string message, string urlDoc, Exception exception)
        : this(message, urlDoc)
    {
        Exception = exception;
    }

    /// <summary>
    /// Constructeur de base pour l'erreur avec une erreur interne.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="urlDoc"></param>
    /// <param name="innerError"></param>
    protected Error(string message, string urlDoc, Error innerError)
        : this(message, urlDoc)
    {
        InnerError = innerError;
    }

    /// <summary>
    /// Constructeur de base pour l'erreur avec une erreur interne.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerError"></param>
    protected Error(string message, Error innerError)
        : this(message)
    {
        InnerError = innerError;
    }

    /// <summary>
    /// Ajouter une données pour l'Erreur, utile pour le log.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddData(string key, object value)
    {
        Data.Add(key, value);
    }

    /// <summary>
    /// Serialise les données sous format JSON
    /// 
    /// Exemple : 
    /// {
    ///     "Data" : {
    ///         "key1" : "value",
    ///         "key2" : "otherValue"
    ///     }
    /// }
    /// </summary>
    public string SerializeData()
    {
        Dictionary<string, object> result = new()
        {
            { "Data", Data }
        };

        return JsonSerializer.Serialize(result);
    }
}
