using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductController : ControllerBase
	{
		private readonly IProductService _service;

		public ProductController(IProductService service)
		{
			_service = service;
		}

		// Basic CRUD using strongly-typed Product and fn_product_master

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var products = await _service.GetAllAsync();
			return Ok(products);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var product = await _service.GetByIdAsync(id);
			if (product == null) return NotFound();
			return Ok(product);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Product product)
		 {
			await _service.InsertAsync(product);
			return Ok();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] Product product)
		{
			product.ProductId = id;
			if (id != product.ProductId) return BadRequest();
			await _service.UpdateAsync(product);
			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			await _service.DeleteAsync(id);
			return Ok();
		}

		// Existing endpoint to fetch products with joined detail information
		[HttpGet("details")]
		public async Task<ActionResult<IEnumerable<ProductWithDetails>>> GetAllWithDetails()
		{
			var result = await _service.GetAllWithDetailsAsync();
			return Ok(result);
		}
	}
}
