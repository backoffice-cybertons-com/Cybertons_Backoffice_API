using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.RepositoryInterfaces;
using System.Data;
using Npgsql;
using System.Dynamic;
using System;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class PartyTypeRepository : BaseDAL, IPartyTypeRepository
    {
        public PartyTypeRepository(IConfiguration configuration, ITenantProvider tenantProvider) 
            : base(configuration, tenantProvider) { }

        public async Task<IEnumerable<dynamic>> GetAllActiveAsync(int companyId)
        {
            string query = "SELECT * FROM public.fn_get_all_party_types(@p_company_id)";
            
            return await ExecuteReaderAsync(query, cmd => {
                cmd.Parameters.AddWithValue("p_company_id", companyId);
            }, reader => MapToDynamic(reader), CommandType.Text);
        }

        private dynamic MapToDynamic(NpgsqlDataReader reader)
        {
            var expando = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expando;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                // Convert snake_case to PascalCase
                var parts = name.Split('_');
                var pascalName = string.Join("", parts.Select(s => s.Length > 0 ? char.ToUpper(s[0]) + s.Substring(1).ToLower() : ""));
                
                expandoDict.Add(pascalName, reader.IsDBNull(i) ? null! : reader.GetValue(i));
            }

            return expando;
        }
    }
}
