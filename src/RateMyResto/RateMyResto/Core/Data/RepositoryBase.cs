using Microsoft.Data.SqlClient;
using RateMyResto.Features.Shared.Configurations;
using System.Data;
using System.Text;
using System.Text.Json;

namespace RateMyResto.Core.Data;

public abstract class RepositoryBase<T>
    where T : class
{
    /// <summary>
    /// SQL full connection string from ConfigurationBase
    /// </summary>
    protected readonly string _connectionString;

    /// <summary>
    /// SQL COmmand global timeout for the DAO manager in seconds
    /// </summary>
    protected readonly int _commandTimeout;

    protected ILogger<T> _logger;

    /// <summary>
    /// Default Ctor
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="commandTimeout"></param>
    protected RepositoryBase(IApplicationSettings config, ILogger<T> logger, int commandTimeout = 0)
    {
        _connectionString = config.GetSqlServerConnection();
        _commandTimeout = commandTimeout;
        _logger = logger;
    }

    /// <summary>
    /// Exécute une procédure stockée et renvoie true si l'exécution ne génère aucune erreur
    /// </summary>
    /// <param name="procName"></param>
    /// <param name="parameters"></param>
    /// <param name="sqlTimeout">timeout setting for request in seconds</param>
    /// <returns>true if succeeded</returns>
    protected async Task<ResultOf> ExecuteNonQueryStoredProcedureAsync(string procName,
                                                                        IDbDataParameter[]? parameters = null,
                                                                        int sqlTimeout = 0)
    {
        try
        {
            await using SqlConnection sqlConnection = new (_connectionString);
            if (sqlConnection.State is not ConnectionState.Open)
            {
                await sqlConnection.OpenAsync()
                                   .ConfigureAwait(false);
            }

            await using var sqlCommand = new SqlCommand(procName, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (sqlTimeout > 0)
            {
                sqlCommand.CommandTimeout = sqlTimeout;
            }
            else if (_commandTimeout > 0)
            {
                sqlCommand.CommandTimeout = _commandTimeout;
            }
            if (parameters is not null)
            {
                sqlCommand.Parameters.AddRange(parameters);
            }

            await sqlCommand.ExecuteNonQueryAsync()
                            .ConfigureAwait(false);

            return ResultOf.Success();
        }
        catch (Exception ex)
        {
            SqlServerError sqlError = new($"Erreur sur la PS : {procName}", ex);
            _logger.LogError(sqlError);

            return ResultOf.Failure(sqlError);
        }
    }

    /// <summary>
    /// Exécute une procédure stockée et renvoie le model
    /// </summary>
    /// <typeparam name="E">Entité de retour</typeparam>
    /// <param name="procName"></param>
    /// <param name="mapDataDelegate"></param>
    /// <param name="parameters"></param>
    /// <param name="sqlTimeout"></param>
    /// <returns></returns>
    protected async Task<ResultOf<E>> ExecuteStoredProcedureAsync<E>(string procName,
                                                                    Func<SqlDataReader, Task<E>> mapDataDelegate,
                                                                    IDbDataParameter[]? parameters = null,
                                                                    int sqlTimeout = 0)
        where E : class
    {
        try
        {
            await using SqlConnection sqlConnection = new (_connectionString);
            if (sqlConnection.State is not ConnectionState.Open)
            {
                await sqlConnection.OpenAsync()
                                   .ConfigureAwait(false);
            }

            await using SqlCommand sqlCommand = new (procName, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (parameters is not null)
            {
                sqlCommand.Parameters.AddRange(parameters);
            }

            if (sqlTimeout > 0)
            {
                sqlCommand.CommandTimeout = sqlTimeout;
            }
            else if (_commandTimeout > 0)
            {
                sqlCommand.CommandTimeout = _commandTimeout;
            }

            await using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync()
                                                               .ConfigureAwait(false);

            E result = await mapDataDelegate(reader).ConfigureAwait(false);

            return ResultOf.Success<E>(result);
        }
        catch (Exception ex)
        {
            SqlServerError sqlError = new($"Erreur sur la PS : {procName}", ex);
            _logger.LogError(sqlError);

            return ResultOf.Failure<E>(sqlError);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="procName"></param>
    /// <param name="mapDataDelegate"></param>
    /// <param name="parameters"></param>
    /// <param name="sqlTimeout"></param>
    /// <returns></returns>
    protected async Task<ResultOf<int>> ExecuteStoredProcedureAsync(string procName,
                                                                    IDbDataParameter[]? parameters = null,
                                                                    int sqlTimeout = 0)
    {
        try
        {
            await using SqlConnection sqlConnection = new (_connectionString);
            if (sqlConnection.State is not ConnectionState.Open)
            {
                await sqlConnection.OpenAsync()
                                   .ConfigureAwait(false);
            }

            await using SqlCommand sqlCommand = new (procName, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (parameters is not null)
                sqlCommand.Parameters.AddRange(parameters);

            if (sqlTimeout > 0)
                sqlCommand.CommandTimeout = sqlTimeout;
            else if (_commandTimeout > 0)
                sqlCommand.CommandTimeout = _commandTimeout;

            await using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync()
                                                               .ConfigureAwait(false);
            int result = 0;

            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    result = reader.GetInt32(0);
                }
            }

            return ResultOf.Success(result);
        }
        catch (Exception ex)
        {
            SqlServerError sqlError = new($"Erreur sur la PS : {procName}", ex);
            _logger.LogError(sqlError);

            return ResultOf.Failure<int>(sqlError);
        }
    }

    /// <summary>
    /// Permet d'exécuter une procédure stockée qui retourne le résultat au format JSON.
    /// </summary>
    /// <typeparam name="E">Entité de retour</typeparam>
    /// <param name="procName"></param>
    /// <param name="parameters"></param>
    /// <param name="sqlTimeout"></param>
    /// <returns></returns>
    protected async Task<ResultOf<E>> ExecuteStoredProcedureWithJsonResultAsync<E>(string procName,
                                                                              IDbDataParameter[]? parameters = null,
                                                                              int sqlTimeout = 0)
    {
        try
        {
            await using (SqlConnection sqlConnection = new (_connectionString))
            {
                if (sqlConnection.State is not ConnectionState.Open)
                {
                    await sqlConnection.OpenAsync()
                                        .ConfigureAwait(false);
                }

                await using SqlCommand sqlCommand = new (procName, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                if (parameters is not null)
                    sqlCommand.Parameters.AddRange(parameters);

                if (sqlTimeout > 0)
                    sqlCommand.CommandTimeout = sqlTimeout;
                else if (_commandTimeout > 0)
                    sqlCommand.CommandTimeout = _commandTimeout;

                StringBuilder jsonResult = new StringBuilder();

                await using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync()
                                                         .ConfigureAwait(false);
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        jsonResult.Append(reader.GetValue(0));
                    }

                    string jsonContent = jsonResult.ToString();

                    if (!string.IsNullOrWhiteSpace(jsonContent))
                    {
                        E? result = JsonSerializer.Deserialize<E>(jsonContent);

                        if (result is null)
                        {
                            SqlServerError error = new($"Erreur de désérialisation JSON sur la PS : {procName}");
                            return ResultOf.Failure<E>(error);
                        }

                        return ResultOf.Success<E>(result);
                    }
                }

                NotFoundError errorNotFound = new($"Aucun résultat JSON sur la PS : {procName}");
                return ResultOf.Failure<E>(errorNotFound);
            }
        }
        catch (Exception ex)
        {
            SqlServerError sqlError = new($"Erreur sur la PS : {procName}", ex);
            _logger.LogError(sqlError);

            return ResultOf.Failure<E>(sqlError);
        }
    }

    /// <summary>
    /// Permet d'exécuter une procédure stockée qui retourne un booléen.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="procName"></param>
    /// <param name="parameters"></param>
    /// <param name="sqlTimeout"></param>
    /// <returns></returns>
    protected async Task<ResultOf<bool>> ExecuteStoredProcedureWithBooleanResult(string procName, 
                                                                                IDbDataParameter[]? parameters = null, 
                                                                                int sqlTimeout = 0)
    {
        try
        {
            await using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                if (sqlConnection.State is not ConnectionState.Open)
                {
                    await sqlConnection.OpenAsync()
                                        .ConfigureAwait(false);
                }

                await using var sqlCommand = new SqlCommand(procName, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                if (parameters is not null)
                {
                    sqlCommand.Parameters.AddRange(parameters);
                }

                if (sqlTimeout > 0)
                    sqlCommand.CommandTimeout = sqlTimeout;
                else if (_commandTimeout > 0)
                    sqlCommand.CommandTimeout = _commandTimeout;

                object? result = await sqlCommand.ExecuteScalarAsync()
                                                .ConfigureAwait(false);

                return ResultOf.Success(Convert.ToBoolean(result));
            }
        }
        catch (Exception ex)
        {
            SqlServerError sqlError = new($"Erreur sur la PS : {procName}", ex);
            _logger.LogError(sqlError);

            return ResultOf.Failure<bool>(sqlError);
        }
    }

    /// <summary>
    /// Permet d'exécuter une procédure stockée qui retourne un entier.
    /// </summary>
    /// <param name="procName"></param>
    /// <param name="parameters"></param>
    /// <param name="sqlTimeout"></param>
    /// <returns></returns>
    protected async Task<ResultOf<int>> ExecuteStoredProcedureWithIntegerResult(string procName, 
                                                                        IDbDataParameter[]? parameters = null, 
                                                                        int sqlTimeout = 0)
    {
        try
        {
            await using (SqlConnection sqlConnection = new (_connectionString))
            {
                if (sqlConnection.State is not ConnectionState.Open)
                {
                    await sqlConnection.OpenAsync()
                                        .ConfigureAwait(false);
                }

                await using SqlCommand sqlCommand = new (procName, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                if (parameters is not null)
                {
                    sqlCommand.Parameters.AddRange(parameters);
                }

                if (sqlTimeout > 0)
                    sqlCommand.CommandTimeout = sqlTimeout;
                else if (_commandTimeout > 0)
                    sqlCommand.CommandTimeout = _commandTimeout;

                await using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync()
                                                         .ConfigureAwait(false);
                int result = -1;
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        result = reader.GetInt32(0);
                    }
                }

                return ResultOf.Success<int>(result);
            }
        }
        catch (Exception ex)
        {
            SqlServerError sqlError = new($"Erreur sur la PS : {procName}", ex);
            _logger.LogError(sqlError);

            return ResultOf.Failure<int>(sqlError);
        }
    }

    /// <summary>
    /// Execute la procédure stockée et retourne un INT
    /// </summary>
    /// <param name="procName"></param>
    /// <param name="parameters"></param>
    /// <param name="sqlTimeout"></param>
    /// <returns></returns>
    protected async Task<ResultOf<object?>> ExecuteScalarStoredProcedureAsync(string procName,
                                                                    IDbDataParameter[]? parameters = null,
                                                                    int sqlTimeout = 0)
    {
        try
        {
            await using (SqlConnection sqlConnection = new (_connectionString))
            {
                if (sqlConnection.State is not ConnectionState.Open)
                {
                    await sqlConnection.OpenAsync()
                                       .ConfigureAwait(false);
                }

                await using SqlCommand sqlCommand = new (procName, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                if (parameters is not null)
                    sqlCommand.Parameters.AddRange(parameters);

                if (sqlTimeout > 0)
                    sqlCommand.CommandTimeout = sqlTimeout;
                else if (_commandTimeout > 0)
                    sqlCommand.CommandTimeout = _commandTimeout;

                object? result = await sqlCommand.ExecuteScalarAsync()
                                        .ConfigureAwait(false);

                return ResultOf.Success(result);
            }
        }
        catch (Exception ex)
        {
            SqlServerError sqlError = new($"Erreur sur la PS : {procName}", ex);
            _logger.LogError(sqlError);

            return ResultOf.Failure<object?>(sqlError);
        }
    }

    /// <summary>
    /// Execute la requête SQL et retourne une class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="mapDataDelegate"></param>
    /// <param name="sqlTimeout"></param>
    /// <returns></returns>
    protected async Task<ResultOf<E>> ExecuteQueryAsync<E>(string query, Func<SqlDataReader, Task<E>> mapDataDelegate, int sqlTimeout = 0)
        where E : class
    {
        try
        {
            await using (SqlConnection sqlConnection = new (_connectionString))
            {
                if (sqlConnection.State is not ConnectionState.Open)
                {
                    await sqlConnection.OpenAsync()
                                        .ConfigureAwait(false);
                }

                await using SqlCommand sqlCommand = new (query, sqlConnection);
                sqlCommand.CommandType = CommandType.Text;

                if (sqlTimeout > 0)
                    sqlCommand.CommandTimeout = sqlTimeout;
                else if (_commandTimeout > 0)
                    sqlCommand.CommandTimeout = _commandTimeout;

                await using var reader = await sqlCommand.ExecuteReaderAsync()
                                                         .ConfigureAwait(false);

                E result = await mapDataDelegate(reader).ConfigureAwait(false);

                return ResultOf.Success(result);
            }
        }
        catch (Exception ex)
        {
            SqlServerError sqlError = new($"Erreur sur l'exécution de la requête dans : {nameof(ExecuteQueryAsync)}", ex);
            _logger.LogError(sqlError);

            return ResultOf.Failure<E>(sqlError);
        }
    }

    /// <summary>
    /// Execute une requête SQL et retourne une class avec un désérialisation JSON
    /// </summary>
    /// <typeparam name="E">Entité de retour</typeparam>
    /// <param name="query"></param>
    /// <param name="sqlTimeout"></param>
    /// <returns></returns>
    protected async Task<ResultOf<E?>> ExecuteQueryAsync<E>(string query, int sqlTimeout = 0)
        where E : class
    {
        try
        {
            E? result = null;

            await using (SqlConnection sqlConnection = new (_connectionString))
            {
                if (sqlConnection.State is not ConnectionState.Open)
                {
                    await sqlConnection.OpenAsync()
                                       .ConfigureAwait(false);
                }

                await using SqlCommand sqlCommand = new (query, sqlConnection);
                sqlCommand.CommandType = CommandType.Text;

                if (sqlTimeout > 0)
                    sqlCommand.CommandTimeout = sqlTimeout;
                else if (_commandTimeout > 0)
                    sqlCommand.CommandTimeout = _commandTimeout;

                StringBuilder jsonResult = new StringBuilder();

                await using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync()
                                                         .ConfigureAwait(false);
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        jsonResult.Append(reader.GetValue(0).ToString());
                    }

                    string jsonContent = jsonResult.ToString();

                    if (!string.IsNullOrWhiteSpace(jsonContent))
                    {
                        result = JsonSerializer.Deserialize<E>(jsonContent);
                    }
                }

                return ResultOf.Success(result);
            }
        }
        catch (Exception ex)
        {
            SqlServerError sqlError = new($"Erreur sur l'exécution de la requête dans : {nameof(ExecuteQueryAsync)}", ex);
            _logger.LogError(sqlError);

            return ResultOf.Failure<E?>(sqlError);
        }
    }

    /// <summary>
    /// Execute une requête SQL et retourne un JSON
    /// </summary>
    /// <param name="query"></param>
    /// <param name="sqlTimeout"></param>
    /// <returns></returns>
    protected async Task<ResultOf<string>> ExecuteQueryWithJsonResultAsync(string query, int sqlTimeout = 0)
    {
        try
        {
            await using (SqlConnection sqlConnection = new (_connectionString))
            {
                if (sqlConnection.State is not ConnectionState.Open)
                {
                    await sqlConnection.OpenAsync()
                                       .ConfigureAwait(false);
                }

                await using SqlCommand sqlCommand = new (query, sqlConnection);
                sqlCommand.CommandType = CommandType.Text;

                if (sqlTimeout > 0)
                    sqlCommand.CommandTimeout = sqlTimeout;
                else if (_commandTimeout > 0)
                    sqlCommand.CommandTimeout = _commandTimeout;

                StringBuilder jsonResult = new StringBuilder();

                await using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync()
                                                         .ConfigureAwait(false);
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        jsonResult.Append(reader.GetValue(0).ToString());
                    }

                    string result = jsonResult.ToString();

                    return ResultOf.Success(result);
                }

                return ResultOf.Success(string.Empty);
            }
        }
        catch (Exception ex)
        {
            SqlServerError sqlError = new($"Erreur sur l'exécution de la requête dans : {nameof(ExecuteQueryWithJsonResultAsync)}", ex);
            _logger.LogError(sqlError);

            return ResultOf.Failure<string>(sqlError);
        }
    }

    /// <summary>
    /// Exécute une procédure stockée avec un mapper personnalisé qui peut retourner un type valeur (int, bool, etc.)
    /// </summary>
    /// <typeparam name="TResult">Type de résultat (peut être un type valeur)</typeparam>
    /// <param name="procName">Nom de la procédure stockée</param>
    /// <param name="mapDataDelegate">Délégué pour mapper le résultat</param>
    /// <param name="parameters">Paramètres SQL</param>
    /// <param name="sqlTimeout">Timeout en secondes</param>
    /// <returns>Résultat de l'exécution</returns>
    protected async Task<ResultOf<TResult>> ExecuteStoredProcedureWithValueTypeAsync<TResult>(string procName,
                                                                                            Func<SqlDataReader, Task<TResult>> mapDataDelegate,
                                                                                            IDbDataParameter[]? parameters = null,
                                                                                            int sqlTimeout = 0)
    {
        try
        {
            await using SqlConnection sqlConnection = new (_connectionString);
            if (sqlConnection.State is not ConnectionState.Open)
            {
                await sqlConnection.OpenAsync()
                                   .ConfigureAwait(false);
            }

            await using SqlCommand sqlCommand = new (procName, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (parameters is not null)
            {
                sqlCommand.Parameters.AddRange(parameters);
            }

            if (sqlTimeout > 0)
            {
                sqlCommand.CommandTimeout = sqlTimeout;
            }
            else if (_commandTimeout > 0)
            {
                sqlCommand.CommandTimeout = _commandTimeout;
            }

            await using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync()
                                                               .ConfigureAwait(false);

            TResult result = await mapDataDelegate(reader).ConfigureAwait(false);

            return ResultOf.Success(result);
        }
        catch (Exception ex)
        {
            SqlServerError sqlError = new($"Erreur sur la PS : {procName}", ex);
            _logger.LogError(sqlError);

            return ResultOf.Failure<TResult>(sqlError);
        }
    }

    #region SqlParamters


    /// <summary>
    /// Permet de retourner un SqlParamter de type Int
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected SqlParameter GetSqlParameterInt(string parameterName, int? value)
    {
        return new SqlParameter(parameterName, SqlDbType.Int) { Value = value };
    }

    /// <summary>
    /// Permet de retourner un SqlParamter de type Short
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected SqlParameter GetSqlParameterTinyInt(string parameterName, short? value)
    {
        return new SqlParameter(parameterName, SqlDbType.TinyInt) { Value = value };
    }

    /// <summary>
    /// Permet de retourner un SqlParamter de type Varchar
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected SqlParameter GetSqlParameterVarchar(string parameterName, string? value)
    {
        return new SqlParameter(parameterName, SqlDbType.VarChar) { Value = value };
    }

    /// <summary>
    /// Permet de retourner un SqlParamter de type UniqueIdentifier
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected SqlParameter GetSqlParameterUniqueIdentifier(string parameterName, Guid? value)
    {
        return new SqlParameter(parameterName, SqlDbType.UniqueIdentifier) { Value = value };
    }

    /// <summary>
    /// Permet de retourner un SqlParamter de type Decimal
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected SqlParameter GetSqlParameterDecimal(string parameterName, decimal? value)
    {
        return new SqlParameter(parameterName, SqlDbType.Decimal) { Value = value };
    }

    /// <summary>
    /// Permet de retourner un SqlParameter de type NVarChar (pour les chaînes Unicode, notamment JSON)
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected SqlParameter GetSqlParameterNVarchar(string parameterName, string? value)
    {
        return new SqlParameter(parameterName, SqlDbType.NVarChar) { Value = value };
    }

    /// <summary>
    /// Permet de retourner un SqlParameter de type NVarChar (pour les chaînes Unicode, notamment JSON)
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected SqlParameter GetSqlParameterNVarchar(string parameterName, string? value, int size)
    {
        return new SqlParameter(parameterName, SqlDbType.NVarChar, size) 
        { 
            Value = value
        };
    }

    /// <summary>
    /// Permet de retourner un SqlParameter de type Bit (booléen)
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected SqlParameter GetSqlParameterBit(string parameterName, bool? value)
    {
        return new SqlParameter(parameterName, SqlDbType.Bit) { Value = value };
    }

    /// <summary>
    /// Permet de retourner un SqlParameter de type DateTime
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected SqlParameter GetSqlParameterDateTime(string parameterName, DateTime? value)
    {
        return new SqlParameter(parameterName, SqlDbType.DateTime) { Value = value };
    }

    /// <summary>
    /// Permet de retourner un SqlParameter de type DateTime2
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected SqlParameter GetSqlParameterDateTime2(string parameterName, DateTime? value)
    {
        return new SqlParameter(parameterName, SqlDbType.DateTime2) { Value = value };
    }

    /// <summary>
    /// Permet de retourner un SqlParameter de type DateTime2
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected SqlParameter GetSqlParameterDate(string parameterName, DateOnly? value)
    {
        return new SqlParameter(parameterName, SqlDbType.Date) { Value = value };
    }

    #endregion

}
