using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using SoftBook.Infrastructure.Interfaces;
using System;

namespace SoftBook.Domain.Data
{
  /// <summary>
  /// Lightweight ADO.NET base class used by repositories.
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

    protected async Task ExecuteNonQueryAsync(string storedProcedure, Action<SqlCommand> buildParameters)
    {
      await using var connection = new SqlConnection(GetConnectionString());
      await connection.OpenAsync();

      await using var command = new SqlCommand(storedProcedure, connection)
      {
        CommandType = CommandType.StoredProcedure,
          CommandTimeout = 60
      };

      buildParameters(command);
      await command.ExecuteNonQueryAsync();
    }

    protected async Task<List<T>> ExecuteReaderAsync<T>(string storedProcedure, Action<SqlCommand> buildParameters, Func<SqlDataReader, T> map)
    {
      var results = new List<T>();

      await using var connection = new SqlConnection(GetConnectionString());
      await connection.OpenAsync();

      await using var command = new SqlCommand(storedProcedure, connection)
      {
          CommandType = CommandType.StoredProcedure,
          CommandTimeout = 60
      };

      buildParameters(command);

      await using var reader = await command.ExecuteReaderAsync();  
      while (await reader.ReadAsync())
      {
        results.Add(map(reader));
      }

      return results;
    }

    protected async Task<T?> ExecuteSingleReaderAsync<T>(string storedProcedure, Action<SqlCommand> buildParameters, Func<SqlDataReader, T> map)
    {
      await using var connection = new SqlConnection(GetConnectionString());
      await connection.OpenAsync();

      await using var command = new SqlCommand(storedProcedure, connection)
      {
          CommandType = CommandType.StoredProcedure,
          CommandTimeout = 60
      };

      buildParameters(command);

      await using var reader = await command.ExecuteReaderAsync();
      if (await reader.ReadAsync())
      {
        return map(reader);
      }

      return default;
    }

    protected async Task<List<T>> ExecuteRawQueryAsync<T>(string sqlQuery, Action<SqlCommand> buildParameters, Func<SqlDataReader, T> map)
    {
      var results = new List<T>();

      await using var connection = new SqlConnection(GetConnectionString());
      await connection.OpenAsync();

      await using var command = new SqlCommand(sqlQuery, connection)
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

        //---Newly Added Overload for sql transaction
        protected async Task ExecuteNonQueryAsync(string storedProcedure,Action<SqlCommand> buildParameters,SqlTransaction transaction)
        {
            var connection = transaction.Connection!;

            await using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure,
                Transaction = transaction,
                CommandTimeout = 60
            };

            buildParameters(command);
            await command.ExecuteNonQueryAsync();
        }


        //----Execute AddScalerAsync
        protected async Task<T?> ExecuteScalarAsync<T>(string storedProcedure,Action<SqlCommand> buildParameters)
        {
            await using var connection = new SqlConnection(GetConnectionString());
            await connection.OpenAsync();

            await using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 60
            };

            buildParameters(command);

            var result = await command.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
                return default;

            return (T)Convert.ChangeType(result, typeof(T));
        }


        protected void LogSql(string spName)
        {
            // optional logging
        }


        protected string SafeGetString(SqlDataReader reader, string columnName, string defaultValue = "")
    {
      try
      {
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return defaultValue;

        var value = reader.GetValue(ordinal); // avoids InvalidCastException
        return value == null ? defaultValue : Convert.ToString(value);
      }
      catch (IndexOutOfRangeException)
      {
        return defaultValue;
      }
    }

    protected int SafeGetInt(SqlDataReader reader, string columnName, int defaultValue = 0)
    {
      try
      {
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return defaultValue;
        
        var value = reader.GetValue(ordinal);
        if (value is int i) return i;
        if (value is long l) return (int)l;
        if (value is decimal d) return (int)d;
        
        return Convert.ToInt32(value);
      }
      catch (IndexOutOfRangeException)
      {
        return defaultValue;
      }
    }

    protected bool? SafeGetBool(SqlDataReader reader, string columnName, bool? defaultValue = null)
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

    protected DateTime? SafeGetDateTime(SqlDataReader reader, string columnName, DateTime? defaultValue = null)
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
  }
}
