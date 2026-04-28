using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductController : ControllerBase
	{
		private readonly IProductService _service;
		private readonly ITenantProvider _tenantProvider;

		public ProductController(IProductService service, ITenantProvider tenantProvider)
		{
			_service = service;
			_tenantProvider = tenantProvider;
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
			var userId = _tenantProvider.UserId;
			var dbResult = await _service.InsertAsync(product, userId);
			if (!dbResult.Success)
			{
				return BadRequest(dbResult);
			}
			return Ok();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] Product product)
		{
			product.ProductId = id;
			var userId = _tenantProvider.UserId;
			var dbResult = await _service.UpdateAsync(product, userId);
			if (!dbResult.Success)
			{
				return BadRequest(dbResult);
			}
			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var userId = _tenantProvider.UserId;
			var dbResult = await _service.DeleteAsync(id, userId);
			if (!dbResult.Success)
			{
				return BadRequest(dbResult);
			}
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
