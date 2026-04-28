using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface ITenantProvider
    {
        int? CompanyId { get; set; }
        int? FiscalYearId { get; set; }
        int? UserId { get; set; }
        string? ConnectionString { get; set; }
    }

}
