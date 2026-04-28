using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository _repository;

		public ProductService(IProductRepository repository)
		{
			_repository = repository;
		}

		public async Task<List<Product>> GetAllAsync()
		{
			return await _repository.GetAllAsync();
		}

		public async Task<Product?> GetByIdAsync(int id)
		{
			return await _repository.GetByIdAsync(id);
		}

		public async Task<DbResult> InsertAsync(Product entity, int? userId = null)
		{
			return await _repository.InsertAsync(entity, userId);
		}

		public async Task<DbResult> UpdateAsync(Product entity, int? userId = null)
		{
			return await _repository.UpdateAsync(entity, userId);
		}

		public async Task<DbResult> DeleteAsync(int id, int? userId = null)
		{
			return await _repository.DeleteAsync(id, userId);
		}

		public async Task<List<ProductWithDetails>> GetAllWithDetailsAsync()
		{
			return await _repository.GetAllWithDetailsAsync();
		}
	}
}
