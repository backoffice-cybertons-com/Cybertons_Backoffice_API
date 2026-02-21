using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
	public interface IProductRepository : IRepository<Product>
	{
		Task<List<ProductWithDetails>> GetAllWithDetailsAsync();

	}
}
