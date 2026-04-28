using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using SoftBook.Domain.Data;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.RepositoryInterfaces;
using System.Dynamic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Linq;
using SoftBook.Domain.DTOs;
using System;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class MasterRepository : BaseDAL, IMasterRepository
    {
        public MasterRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<IEnumerable<dynamic>> GetAllAsync(string masterName, int? filterId = null, int? companyId = null)
        {
            // PostgreSQL function must be called via SELECT * FROM ...
            // Using named notation (:=) to be extra safe
            string query = "SELECT * FROM public.sp_master_universal(p_master_name := @p_master_name, p_action := @p_action, p_company_id := @p_company_id, p_id := @p_id, p_json_data := @p_json_data)";
            
            Console.WriteLine($"[MasterRepository] GetAllAsync: masterName={masterName}, companyId={companyId ?? _tenantProvider?.CompanyId ?? 0}, userId={_tenantProvider?.UserId}");
            return await ExecuteReaderAsync(query, cmd =>
            {
                cmd.Parameters.Add(new NpgsqlParameter("p_master_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = masterName });
                cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = companyId ?? _tenantProvider?.CompanyId ?? 0 });
                cmd.Parameters.Add(new NpgsqlParameter("p_json_data", NpgsqlTypes.NpgsqlDbType.Jsonb) { Value = DBNull.Value });

                if (filterId.HasValue)
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p_action", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = "SELECT_BY_FILTER" });
                    cmd.Parameters.Add(new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = filterId.Value });
                }
                else
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p_action", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = "SELECT" });
                    cmd.Parameters.Add(new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = DBNull.Value });
                }
            }, reader => MapToDynamic(reader), CommandType.Text);
        }

        public async Task<dynamic?> GetByIdAsync(string masterName, int id, int? companyId = null)
        {
            string query = "SELECT * FROM public.sp_master_universal(p_master_name := @p_master_name, p_action := @p_action, p_company_id := @p_company_id, p_id := @p_id, p_json_data := @p_json_data)";

            return await ExecuteSingleReaderAsync(query, cmd =>
            {
                cmd.Parameters.Add(new NpgsqlParameter("p_master_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = masterName });
                cmd.Parameters.Add(new NpgsqlParameter("p_action", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = "SELECT_BY_ID" });
                cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = companyId ?? _tenantProvider?.CompanyId ?? 0 });
                cmd.Parameters.Add(new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = id });
                cmd.Parameters.Add(new NpgsqlParameter("p_json_data", NpgsqlTypes.NpgsqlDbType.Jsonb) { Value = DBNull.Value });
            }, reader => MapToDynamic(reader), CommandType.Text);
        }

        public async Task<DbResult> InsertAsync(string masterName, string jsonData, int? userId = null)
        {
            try
            {
                var snakeJson = ToSnakeCaseJson(jsonData);
                string query = "SELECT * FROM public.sp_master_universal(p_master_name := @p_master_name, p_action := @p_action, p_company_id := @p_company_id, p_id := @p_id, p_json_data := @p_json_data, p_user_id := @p_user_id)";
                
                var data = await ExecuteSingleReaderAsync(query, cmd =>
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p_master_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = masterName });
                    cmd.Parameters.Add(new NpgsqlParameter("p_action", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = "INSERT" });
                    cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = _tenantProvider?.CompanyId ?? 0 });
                    cmd.Parameters.Add(new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = DBNull.Value });
                    cmd.Parameters.Add(new NpgsqlParameter("p_json_data", NpgsqlTypes.NpgsqlDbType.Jsonb) { Value = snakeJson });
                    cmd.Parameters.Add(new NpgsqlParameter("p_user_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (object)userId ?? DBNull.Value });
                }, reader => MapToDynamic(reader), CommandType.Text);

                return DbResult.Ok(data);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<DbResult> UpdateAsync(string masterName, int id, string jsonData, int? userId = null)
        {
            try
            {
                var snakeJson = ToSnakeCaseJson(jsonData);
                string query = "SELECT * FROM public.sp_master_universal(p_master_name := @p_master_name, p_action := @p_action, p_company_id := @p_company_id, p_id := @p_id, p_json_data := @p_json_data, p_user_id := @p_user_id)";

                var data = await ExecuteSingleReaderAsync(query, cmd =>
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p_master_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = masterName });
                    cmd.Parameters.Add(new NpgsqlParameter("p_action", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = "UPDATE" });
                    cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = _tenantProvider?.CompanyId ?? 0 });
                    cmd.Parameters.Add(new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = id });
                    cmd.Parameters.Add(new NpgsqlParameter("p_json_data", NpgsqlTypes.NpgsqlDbType.Jsonb) { Value = snakeJson });
                    cmd.Parameters.Add(new NpgsqlParameter("p_user_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (object)userId ?? DBNull.Value });
                }, reader => MapToDynamic(reader), CommandType.Text);

                return DbResult.Ok(data);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<DbResult> DeleteAsync(string masterName, int id, int? userId = null)
        {
            try
            {
                string query = "SELECT * FROM public.sp_master_universal(p_master_name := @p_master_name, p_action := @p_action, p_company_id := @p_company_id, p_id := @p_id, p_json_data := @p_json_data, p_user_id := @p_user_id)";

                await ExecuteSingleReaderAsync(query, cmd =>
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p_master_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = masterName });
                    cmd.Parameters.Add(new NpgsqlParameter("p_action", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = "DELETE" });
                    cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = _tenantProvider?.CompanyId ?? 0 });
                    cmd.Parameters.Add(new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = id });
                    cmd.Parameters.Add(new NpgsqlParameter("p_json_data", NpgsqlTypes.NpgsqlDbType.Jsonb) { Value = DBNull.Value });
                    cmd.Parameters.Add(new NpgsqlParameter("p_user_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (object)userId ?? DBNull.Value });
                }, reader => reader[0], CommandType.Text);

                return DbResult.Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private dynamic MapToDynamic(NpgsqlDataReader reader)
        {
            // If the result is a single JSON column (from row_to_json)
            var typeName = reader.GetDataTypeName(0).ToLower();
            if (reader.FieldCount == 1 && (typeName == "json" || typeName == "jsonb"))
            {
                var json = reader.GetString(0);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json, options);
                var expando = new ExpandoObject();
                var expandoDict = (IDictionary<string, object>)expando;

                if (dict != null)
                {
                    foreach (var kvp in dict)
                    {
                        // Convert snake_case to PascalCase with special handling for acronyms
                        var parts = kvp.Key.Split('_');
                        var pascalKey = string.Join("", parts.Select(s => 
                        {
                            if (string.IsNullOrEmpty(s)) return "";
                            var upper = s.ToUpper();
                            if (upper == "GST" || upper == "PAN" || upper == "TDS" || upper == "URL" || upper == "MSME") 
                                return upper;
                            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
                        }));

                        // Manual override for special combined cases if needed
                        if (pascalKey == "GstNo") pascalKey = "GSTNO";
                        if (pascalKey == "MsmeRegistered") pascalKey = "MSMERegistered";

                        expandoDict.Add(pascalKey, kvp.Value);
                    }
                }
                return expando;
            }

            // Fallback for standard multi-column results
            var resultExpando = new ExpandoObject();
            var resultDict = (IDictionary<string, object>)resultExpando;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var pascalName = string.Join("", name.Split('_').Select(s => 
                    s.Length > 0 ? char.ToUpper(s[0]) + s.Substring(1).ToLower() : ""));
                resultDict.Add(pascalName, reader.IsDBNull(i) ? null! : reader.GetValue(i));
            }
            return resultExpando;
        }

        private string ToSnakeCaseJson(string json)
        {
            try {
                Console.WriteLine($"[MasterRepository] Input JSON: {json}");
                using var doc = JsonDocument.Parse(json);
                var dict = new Dictionary<string, object>();
                foreach (var prop in doc.RootElement.EnumerateObject()) {
                    var snakeKey = Regex.Replace(prop.Name, "(?<!^)([A-Z])", "_$1").ToLower();
                    dict[snakeKey] = prop.Value.ValueKind switch {
                        JsonValueKind.String => prop.Value.GetString()!,
                        JsonValueKind.Number => prop.Value.GetDouble(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.Null => null,
                        _ => prop.Value.GetRawText()
                    };
                }
                var result = JsonSerializer.Serialize(dict);
                Console.WriteLine($"[MasterRepository] Snake JSON: {result}");
                return result;
            } catch (Exception ex) { 
                Console.WriteLine($"[MasterRepository] JSON Error: {ex.Message}");
                return json; 
            }
        }
    }
}
