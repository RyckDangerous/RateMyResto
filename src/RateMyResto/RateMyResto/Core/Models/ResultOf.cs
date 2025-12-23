using RateMyResto.Core.Models.Errors;
using System.Diagnostics.CodeAnalysis;

namespace RateMyResto.Core.Models;

public class ResultOf
{
    #region Properties

    /// <summary>
    /// Indique si le résultat est en succés
    /// </summary>
    public virtual bool IsSuccess { get; protected set; }

    /// <summary>
    /// Indique s'il y a une erreur.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    public virtual bool HasError => Error is not null;

    /// <summary>
    /// Erreur du résultat.
    /// </summary>
    public virtual Error? Error { get; protected set; }

    #endregion

    #region Constructeurs

    protected ResultOf()
    {
        IsSuccess = true;
        Error = null;
    }

    public ResultOf(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    #endregion

    /// <summary>
    /// Permet de copier l'Exception et le message.
    /// </summary>
    /// <param name="resultOf"></param>
    public void CopyError<X>(ResultOf<X> resultOf)
    {
        IsSuccess = resultOf.IsSuccess;
        Error = resultOf.Error;
    }

    #region Helpers

    /// <summary>
    /// Retourne un ResultOf en Success
    /// </summary>
    /// <returns></returns>
    public static ResultOf Success() => new();

    /// <summary>
    /// Retourne un ResultOf avec une valeur en Success
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ResultOf<T> Success<T>(T value) => new(value);

    /// <summary>
    /// Retourne un ResultOf avec une Erreur
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static ResultOf Failure(Error error) => new(error);

    /// <summary>
    /// Retourne un ResultOf<T> avec une Erreur
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="error"></param>
    /// <returns></returns>
    public static ResultOf<T> Failure<T>(Error error) => new(error);

    #endregion

}

public sealed class ResultOf<T> : ResultOf
{
    /// <summary>
    /// Indique si le résultat est en succés
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public override bool IsSuccess { get; protected set; }

    /// <summary>
    /// Indique s'il y a une erreur.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
    public override bool HasError => Error is not null;

    /// <summary>
    /// Erreur du résultat.
    /// </summary>
    public override Error? Error { get; protected set; }

    /// <summary>
    /// Valeur du resultat
    /// </summary>
    public T? Value { get; private set; }

    /// <summary>
    /// Constructeur d'un ResultOf avec un résultat en succés.
    /// </summary>
    /// <param name="value"></param>
    public ResultOf(T value)
    {
        Value = value;
        IsSuccess = true;
        Error = null;
    }

    /// <summary>
    /// Constructeur d'un ResultOf avec une Erreur.
    /// </summary>
    /// <param name="error"></param>
    public ResultOf(Error error)
    {
        Value = default;
        IsSuccess = false;
        Error = error;
    }
}