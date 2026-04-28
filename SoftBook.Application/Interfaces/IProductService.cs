using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Interfaces
{
	public interface IProductService
	{
		Task<List<Product>> GetAllAsync();
		Task<Product?> GetByIdAsync(int id);
		Task<DbResult> InsertAsync(Product entity, int? userId = null);
		Task<DbResult> UpdateAsync(Product entity, int? userId = null);
		Task<DbResult> DeleteAsync(int id, int? userId = null);

		Task<List<ProductWithDetails>> GetAllWithDetailsAsync();

	}
}
