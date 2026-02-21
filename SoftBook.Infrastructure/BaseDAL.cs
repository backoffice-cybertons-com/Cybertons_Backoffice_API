using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using SoftBook.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.DTOs;

namespace SoftBook.Domain.Data
{
  /// <summary>
  /// Lightweight ADO.NET base class used by repositories (Refactored for PostgreSQL).
  /// </summary>
  public class BaseDAL
  {
    protected string _connectionString;
    protected readonly ITenantProvider? _tenantProvider;

    public BaseDAL(IConfiguration configuration, ITenantProvider tenantProvider)
    {
      _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
      _tenantProvider = tenantProvider;
    }

    public void SetConnectionString(string connectionString)
    {
       if (!string.IsNullOrEmpty(connectionString))
       {
           _connectionString = connectionString;
       }
    }

    protected virtual string GetConnectionString()
    {
       if (_tenantProvider != null && !string.IsNullOrEmpty(_tenantProvider.ConnectionString))
       {
           return _tenantProvider.ConnectionString;
       }
       return _connectionString;
    }

    private void ApplyPostgresFunctionSyntax(NpgsqlCommand command)
    {
        if (command.CommandType == CommandType.StoredProcedure)
        {
            var placeholders = new List<string>();
            foreach (NpgsqlParameter param in command.Parameters)
            {
                placeholders.Add($"{param.ParameterName} := @{param.ParameterName}");
            }
            command.CommandText = $"SELECT * FROM {command.CommandText}({string.Join(", ", placeholders)})";
            command.CommandType = CommandType.Text;
        }
    }

    protected async Task ExecuteNonQueryAsync(string commandText, Action<NpgsqlCommand> buildParameters, CommandType commandType = CommandType.StoredProcedure)
    {
      await using var connection = new NpgsqlConnection(GetConnectionString());
      await connection.OpenAsync();

      await using var command = new NpgsqlCommand(commandText, connection)
      {
        CommandType = commandType,
        CommandTimeout = 60
      };

      buildParameters(command);
      ApplyPostgresFunctionSyntax(command);

      await command.ExecuteNonQueryAsync();
    }

    protected async Task<List<T>> ExecuteReaderAsync<T>(string commandText, Action<NpgsqlCommand> buildParameters, Func<NpgsqlDataReader, T> map, CommandType commandType = CommandType.StoredProcedure)
    {
      var results = new List<T>();

      await using var connection = new NpgsqlConnection(GetConnectionString());
      await connection.OpenAsync();
      Console.WriteLine($"[BaseDAL Diagnostic] Connected to Database: '{connection.Database}' for Query: {commandText}");

      await using var command = new NpgsqlCommand(commandText, connection)
      {
          CommandType = commandType,
          CommandTimeout = 60
      };

      buildParameters(command);
      ApplyPostgresFunctionSyntax(command);

      await using var reader = await command.ExecuteReaderAsync();  
      while (await reader.ReadAsync())
      {
        results.Add(map(reader));
      }

      return results;
    }

    protected async Task<T?> ExecuteSingleReaderAsync<T>(string commandText, Action<NpgsqlCommand> buildParameters, Func<NpgsqlDataReader, T> map, CommandType commandType = CommandType.StoredProcedure)
    {
      await using var connection = new NpgsqlConnection(GetConnectionString());
      await connection.OpenAsync();
      Console.WriteLine($"[BaseDAL Diagnostic] Connected to Database: '{connection.Database}' for {commandText}");

      await using var command = new NpgsqlCommand(commandText, connection)
      {
          CommandType = commandType,
          CommandTimeout = 60
      };

      buildParameters(command);
      ApplyPostgresFunctionSyntax(command);

      await using var reader = await command.ExecuteReaderAsync();
      if (await reader.ReadAsync())
      {
        return map(reader);
      }

      return default;
    }

    protected async Task<List<T>> ExecuteRawQueryAsync<T>(string sqlQuery, Action<NpgsqlCommand> buildParameters, Func<NpgsqlDataReader, T> map)
    {
      var results = new List<T>();

      await using var connection = new NpgsqlConnection(GetConnectionString());
      await connection.OpenAsync();

      await using var command = new NpgsqlCommand(sqlQuery, connection)
      {
        CommandType = CommandType.Text
      };

      buildParameters(command);

      await using var reader = await command.ExecuteReaderAsync();
      while (await reader.ReadAsync())
      {
        results.Add(map(reader));
      }

      return results;
    }

    protected async Task ExecuteNonQueryAsync(string storedProcedure, Action<NpgsqlCommand> buildParameters, NpgsqlTransaction transaction)
    {
        var connection = transaction.Connection!;

        await using var command = new NpgsqlCommand(storedProcedure, connection)
        {
            CommandType = CommandType.StoredProcedure,
            Transaction = transaction,
            CommandTimeout = 60
        };

        buildParameters(command);
        ApplyPostgresFunctionSyntax(command);

        await command.ExecuteNonQueryAsync();
    }

    protected async Task<T?> ExecuteScalarAsync<T>(string storedProcedure, Action<NpgsqlCommand> buildParameters)
    {
        await using var connection = new NpgsqlConnection(GetConnectionString());
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(storedProcedure, connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };

        buildParameters(command);
        ApplyPostgresFunctionSyntax(command);

        var result = await command.ExecuteScalarAsync();
        if (result == null || result == DBNull.Value)
            return default;

        return (T)Convert.ChangeType(result, typeof(T));
    }

    protected string SafeGetString(NpgsqlDataReader reader, string columnName, string defaultValue = "")
    {
      try
      {
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return defaultValue;
        return reader.GetValue(ordinal).ToString() ?? defaultValue;
      }
      catch (IndexOutOfRangeException)
      {
        return defaultValue;
      }
    }

    protected int SafeGetInt(NpgsqlDataReader reader, string columnName, int defaultValue = 0)
    {
      try
      {
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return defaultValue;
        return Convert.ToInt32(reader.GetValue(ordinal));
      }
      catch (IndexOutOfRangeException)
      {
        return defaultValue;
      }
    }

    protected bool? SafeGetBool(NpgsqlDataReader reader, string columnName, bool? defaultValue = null)
    {
      try
      {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? defaultValue : reader.GetBoolean(ordinal);
      }
      catch (IndexOutOfRangeException)
      {
        return defaultValue;
      }
    }

    protected DateTime? SafeGetDateTime(NpgsqlDataReader reader, string columnName, DateTime? defaultValue = null)
    {
      try
      {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? defaultValue : reader.GetDateTime(ordinal);
      }
      catch (IndexOutOfRangeException)
      {
        return defaultValue;
      }
    }

    protected void LogSql(string spName) { }
    private string GetParamPlaceholders(string procedureName) => ""; 

    protected DbResult HandleException(Exception ex)
    {
        if (ex is PostgresException pgEx || (ex.InnerException is PostgresException innerPgEx && (pgEx = innerPgEx) != null))
        {
            Console.WriteLine($"[Database Error] SQLState: {pgEx.SqlState}, Message: {pgEx.MessageText}, Detail: {pgEx.Detail}");

            if (pgEx.SqlState == "23505")
            {
                return DbResult.Failure("Duplicate record found. This entry already exists in the system.", pgEx.SqlState, pgEx.MessageText, pgEx.ConstraintName);
            }
            if (pgEx.SqlState == "23503")
            {
                string message = (pgEx.Detail != null && (pgEx.Detail.Contains("not present in table") || pgEx.Detail.Contains("isPresent=false")))
                    ? "The selected parent record (or ID) does not exist. Please check your IDs."
                    : "This record cannot be deleted because it is being used by another process or record.";
                
                return DbResult.Failure(message, pgEx.SqlState, pgEx.MessageText, pgEx.ConstraintName);
            }
            if (pgEx.SqlState == "P0001") // Custom Raise Exception in PL/pgSQL
            {
                return DbResult.Failure(pgEx.MessageText, pgEx.SqlState);
            }
        }
        Console.WriteLine($"[Database Error] Generic Exception: {ex.Message}");
        return DbResult.Failure("A database error occurred: " + ex.Message);
    }
  }
}
