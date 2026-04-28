using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;

namespace SoftBook.Domain.RepositoryInterfaces
{
	public interface IProductRepository : IRepository<Product>
	{
		Task<List<ProductWithDetails>> GetAllWithDetailsAsync();
		Task<Product?> GetByIdAsync(int companyId, int productId);
		Task<DbResult> InsertAsync(Product product, int? userId = null);
		Task<DbResult> UpdateAsync(Product product, int? userId = null);
		Task<DbResult> DeleteAsync(int id, int? userId = null);
	}
}
