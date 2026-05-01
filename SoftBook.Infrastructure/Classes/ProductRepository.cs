using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using Npgsql;

namespace SoftBook.Infrastructure.Repositories.Classes
{
	public class ProductRepository : BaseDAL, IProductRepository
	{
		public ProductRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

		public async Task<List<Product>> GetAllAsync()
		{
			return await ExecuteReaderAsync("fn_product_master", cmd =>
			{
				cmd.Parameters.AddWithValue("p_action", "GETALL");
				cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
			}, reader =>
			{
				var type = SafeGetString(reader, "product_type");
				if (string.IsNullOrEmpty(type))
					type = SafeGetString(reader, "type");

				var name = SafeGetString(reader, "product_name");
				if (string.IsNullOrEmpty(name))
					name = SafeGetString(reader, "name");

				return new Product
				{
					ProductId = SafeGetInt(reader, "product_id"),
					Type = type,
					Name = name,
					ShortName = SafeGetString(reader, "short_name"),
					MRP = reader["mrp"] != DBNull.Value ? Convert.ToDecimal(reader["mrp"]) : (decimal?)null,
					SellingPrice = reader["selling_price"] != DBNull.Value ? Convert.ToDecimal(reader["selling_price"]) : (decimal?)null,
					SalesAccount = SafeGetString(reader, "sales_account"),
					SalesDescription = SafeGetString(reader, "sales_description"),
					CostPrice = reader["cost_price"] != DBNull.Value ? Convert.ToDecimal(reader["cost_price"]) : (decimal?)null,
					PurchaseAccount = SafeGetString(reader, "purchase_account"),
					PurchaseDescription = SafeGetString(reader, "purchase_description"),
					PreferredVendorId = reader["preferred_vendor_id"] != DBNull.Value ? SafeGetInt(reader, "preferred_vendor_id") : (int?)null,
					Sellable = SafeGetBool(reader, "sellable") ?? false,
					Purchasable = SafeGetBool(reader, "purchasable") ?? false,
					UnitId = reader["unit_id"] != DBNull.Value ? SafeGetInt(reader, "unit_id") : (int?)null,
					HSNId = reader["hsn_id"] != DBNull.Value ? SafeGetInt(reader, "hsn_id") : (int?)null,
					SACId = reader["sac_id"] != DBNull.Value ? SafeGetInt(reader, "sac_id") : (int?)null,
					TaxPreferenceId = reader["tax_preference_id"] != DBNull.Value ? SafeGetInt(reader, "tax_preference_id") : (int?)null,
					TaxGroupId = reader["tax_group_id"] != DBNull.Value ? SafeGetInt(reader, "tax_group_id") : (int?)null,
					ProductGroupId = reader["product_group_id"] != DBNull.Value ? SafeGetInt(reader, "product_group_id") : (int?)null,
					ProductPackingUnit = reader["product_packing_unit"] != DBNull.Value ? SafeGetInt(reader, "product_packing_unit") : (int?)null,
					OpeningQuantity = reader["opening_quantity"] != DBNull.Value ? Convert.ToDecimal(reader["opening_quantity"]) : (decimal?)null,
					OpeningPrice = reader["opening_price"] != DBNull.Value ? Convert.ToDecimal(reader["opening_price"]) : (decimal?)null,
					OpeningAmount = reader["opening_amount"] != DBNull.Value ? Convert.ToDecimal(reader["opening_amount"]) : (decimal?)null,
					PurchaseEffectedAccount = reader["purchase_effected_account"] != DBNull.Value ? SafeGetInt(reader, "purchase_effected_account") : (int?)null,
					SaleEffectedAccount = reader["sale_effected_account"] != DBNull.Value ? SafeGetInt(reader, "sale_effected_account") : (int?)null,
					SellingPrice2 = reader["selling_price_2"] != DBNull.Value ? Convert.ToDecimal(reader["selling_price_2"]) : (decimal?)null,
					SellingPrice3 = reader["selling_price_3"] != DBNull.Value ? Convert.ToDecimal(reader["selling_price_3"]) : (decimal?)null,
					SellingPrice4 = reader["selling_price_4"] != DBNull.Value ? Convert.ToDecimal(reader["selling_price_4"]) : (decimal?)null,
					SellingPrice5 = reader["selling_price_5"] != DBNull.Value ? Convert.ToDecimal(reader["selling_price_5"]) : (decimal?)null,
					MinQuantity = reader["min_quantity"] != DBNull.Value ? Convert.ToDecimal(reader["min_quantity"]) : (decimal?)null,
					MaxQuantity = reader["max_quantity"] != DBNull.Value ? Convert.ToDecimal(reader["max_quantity"]) : (decimal?)null,
					ReOrderQuantity = reader["re_order_quantity"] != DBNull.Value ? Convert.ToDecimal(reader["re_order_quantity"]) : (decimal?)null,
					TaxExemptionId = reader["tax_exemption_id"] != DBNull.Value ? SafeGetInt(reader, "tax_exemption_id") : null
				};
			});
		}

		public async Task<Product?> GetByIdAsync(int id)
		{
			return await GetByIdAsync(_tenantProvider?.CompanyId ?? 0, id);
		}

		public async Task<Product?> GetByIdAsync(int companyId, int id)
		{
			return await ExecuteSingleReaderAsync("fn_product_master",
				cmd =>
				{
					cmd.Parameters.AddWithValue("p_action", "GETBYID");
					cmd.Parameters.AddWithValue("p_product_id", id);
					cmd.Parameters.AddWithValue("p_company_id", companyId);
				},
				reader =>
				{
					var type = SafeGetString(reader, "product_type");
					if (string.IsNullOrEmpty(type))
						type = SafeGetString(reader, "type");

					var name = SafeGetString(reader, "product_name");
					if (string.IsNullOrEmpty(name))
						name = SafeGetString(reader, "name");

					return new Product
					{
						ProductId = SafeGetInt(reader, "product_id"),
						Type = type,
						Name = name,
						ShortName = SafeGetString(reader, "short_name"),
						MRP = reader["mrp"] != DBNull.Value ? Convert.ToDecimal(reader["mrp"]) : (decimal?)null,
						SellingPrice = reader["selling_price"] != DBNull.Value ? Convert.ToDecimal(reader["selling_price"]) : (decimal?)null,
						SalesAccount = SafeGetString(reader, "sales_account"),
						SalesDescription = SafeGetString(reader, "sales_description"),
						CostPrice = reader["cost_price"] != DBNull.Value ? Convert.ToDecimal(reader["cost_price"]) : (decimal?)null,
						PurchaseAccount = SafeGetString(reader, "purchase_account"),
						PurchaseDescription = SafeGetString(reader, "purchase_description"),
						PreferredVendorId = reader["preferred_vendor_id"] != DBNull.Value ? SafeGetInt(reader, "preferred_vendor_id") : (int?)null,
						Sellable = SafeGetBool(reader, "sellable") ?? false,
						Purchasable = SafeGetBool(reader, "purchasable") ?? false,
						UnitId = reader["unit_id"] != DBNull.Value ? SafeGetInt(reader, "unit_id") : (int?)null,
						HSNId = reader["hsn_id"] != DBNull.Value ? SafeGetInt(reader, "hsn_id") : (int?)null,
						SACId = reader["sac_id"] != DBNull.Value ? SafeGetInt(reader, "sac_id") : (int?)null,
						TaxPreferenceId = reader["tax_preference_id"] != DBNull.Value ? SafeGetInt(reader, "tax_preference_id") : (int?)null,
						TaxGroupId = reader["tax_group_id"] != DBNull.Value ? SafeGetInt(reader, "tax_group_id") : (int?)null,
						ProductGroupId = reader["product_group_id"] != DBNull.Value ? SafeGetInt(reader, "product_group_id") : (int?)null,
						ProductPackingUnit = reader["product_packing_unit"] != DBNull.Value ? SafeGetInt(reader, "product_packing_unit") : (int?)null,
						OpeningQuantity = reader["opening_quantity"] != DBNull.Value ? Convert.ToDecimal(reader["opening_quantity"]) : (decimal?)null,
						OpeningPrice = reader["opening_price"] != DBNull.Value ? Convert.ToDecimal(reader["opening_price"]) : (decimal?)null,
						OpeningAmount = reader["opening_amount"] != DBNull.Value ? Convert.ToDecimal(reader["opening_amount"]) : (decimal?)null,
						PurchaseEffectedAccount = reader["purchase_effected_account"] != DBNull.Value ? SafeGetInt(reader, "purchase_effected_account") : (int?)null,
						SaleEffectedAccount = reader["sale_effected_account"] != DBNull.Value ? SafeGetInt(reader, "sale_effected_account") : (int?)null,
						SellingPrice2 = reader["selling_price_2"] != DBNull.Value ? Convert.ToDecimal(reader["selling_price_2"]) : (decimal?)null,
						SellingPrice3 = reader["selling_price_3"] != DBNull.Value ? Convert.ToDecimal(reader["selling_price_3"]) : (decimal?)null,
						SellingPrice4 = reader["selling_price_4"] != DBNull.Value ? Convert.ToDecimal(reader["selling_price_4"]) : (decimal?)null,
						SellingPrice5 = reader["selling_price_5"] != DBNull.Value ? Convert.ToDecimal(reader["selling_price_5"]) : (decimal?)null,
						MinQuantity = reader["min_quantity"] != DBNull.Value ? Convert.ToDecimal(reader["min_quantity"]) : (decimal?)null,
						MaxQuantity = reader["max_quantity"] != DBNull.Value ? Convert.ToDecimal(reader["max_quantity"]) : (decimal?)null,
						ReOrderQuantity = reader["re_order_quantity"] != DBNull.Value ? Convert.ToDecimal(reader["re_order_quantity"]) : (decimal?)null,
						TaxExemptionId = reader["tax_exemption_id"] != DBNull.Value ? SafeGetInt(reader, "tax_exemption_id") : null
					};
				});
		}

		public async Task<DbResult> InsertAsync(Product product, int? userId = null)
		{
			try
			{
				await ExecuteNonQueryAsync("fn_product_master", cmd =>
				{
					cmd.Parameters.AddWithValue("p_action", "INSERT");
					// fn_product_master signature expects p_product_id as 2nd argument (nullable) even for INSERT
					cmd.Parameters.AddWithValue("p_product_id", DBNull.Value);
					cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
					cmd.Parameters.AddWithValue("p_user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_product_type", (object?)product.Type ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_product_name", (object?)product.Name ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_short_name", (object?)product.ShortName ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_mrp", (object?)product.MRP ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_selling_price", (object?)product.SellingPrice ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_sales_account", (object?)product.SalesAccount ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_sales_description", (object?)product.SalesDescription ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_cost_price", (object?)product.CostPrice ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_purchase_account", (object?)product.PurchaseAccount ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_purchase_description", (object?)product.PurchaseDescription ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_preferred_vendor_id", (object?)product.PreferredVendorId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_sellable", product.Sellable);
					cmd.Parameters.AddWithValue("p_purchasable", product.Purchasable);
					cmd.Parameters.AddWithValue("p_unit_id", (object?)product.UnitId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_hsn_id", (object?)product.HSNId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_sac_id", (object?)product.SACId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_tax_preference_id", (object?)product.TaxPreferenceId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_tax_group_id", (object?)product.TaxGroupId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_product_group_id", (object?)product.ProductGroupId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_product_packing_unit", (object?)product.ProductPackingUnit ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_opening_quantity", (object?)product.OpeningQuantity ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_opening_price", (object?)product.OpeningPrice ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_opening_amount", (object?)product.OpeningAmount ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_purchase_effected_account", (object?)product.PurchaseEffectedAccount ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_sale_effected_account", (object?)product.SaleEffectedAccount ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_selling_price_2", (object?)product.SellingPrice2 ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_selling_price_3", (object?)product.SellingPrice3 ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_selling_price_4", (object?)product.SellingPrice4 ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_selling_price_5", (object?)product.SellingPrice5 ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_min_quantity", (object?)product.MinQuantity ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_max_quantity", (object?)product.MaxQuantity ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_re_order_quantity", (object?)product.ReOrderQuantity ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_tax_exemption_id", (object?)product.TaxExemptionId ?? DBNull.Value);
				});
				return DbResult.Ok();
			}
			catch (Exception ex)
			{
				return HandleException(ex);
			}
		}

		async Task IRepository<Product>.InsertAsync(Product entity) => await InsertAsync(entity, null);

		public async Task<DbResult> UpdateAsync(Product product, int? userId = null)
		{
			try
			{
				await ExecuteNonQueryAsync("fn_product_master", cmd =>
				{
					cmd.Parameters.AddWithValue("p_action", "UPDATE");
					cmd.Parameters.AddWithValue("p_product_id", product.ProductId);
					cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
					cmd.Parameters.AddWithValue("p_user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_product_type", (object?)product.Type ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_product_name", (object?)product.Name ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_short_name", (object?)product.ShortName ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_mrp", (object?)product.MRP ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_selling_price", (object?)product.SellingPrice ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_sales_account", (object?)product.SalesAccount ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_sales_description", (object?)product.SalesDescription ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_cost_price", (object?)product.CostPrice ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_purchase_account", (object?)product.PurchaseAccount ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_purchase_description", (object?)product.PurchaseDescription ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_preferred_vendor_id", (object?)product.PreferredVendorId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_sellable", product.Sellable);
					cmd.Parameters.AddWithValue("p_purchasable", product.Purchasable);
					cmd.Parameters.AddWithValue("p_unit_id", (object?)product.UnitId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_hsn_id", (object?)product.HSNId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_sac_id", (object?)product.SACId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_tax_preference_id", (object?)product.TaxPreferenceId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_tax_group_id", (object?)product.TaxGroupId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_product_group_id", (object?)product.ProductGroupId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_product_packing_unit", (object?)product.ProductPackingUnit ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_opening_quantity", (object?)product.OpeningQuantity ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_opening_price", (object?)product.OpeningPrice ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_opening_amount", (object?)product.OpeningAmount ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_purchase_effected_account", (object?)product.PurchaseEffectedAccount ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_sale_effected_account", (object?)product.SaleEffectedAccount ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_selling_price_2", (object?)product.SellingPrice2 ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_selling_price_3", (object?)product.SellingPrice3 ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_selling_price_4", (object?)product.SellingPrice4 ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_selling_price_5", (object?)product.SellingPrice5 ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_min_quantity", (object?)product.MinQuantity ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_max_quantity", (object?)product.MaxQuantity ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_re_order_quantity", (object?)product.ReOrderQuantity ?? DBNull.Value);
					cmd.Parameters.AddWithValue("p_tax_exemption_id", (object?)product.TaxExemptionId ?? DBNull.Value);
				});
				return DbResult.Ok();
			}
			catch (Exception ex)
			{
				return HandleException(ex);
			}
		}

		async Task IRepository<Product>.UpdateAsync(Product entity) => await UpdateAsync(entity, null);

		public async Task<DbResult> DeleteAsync(int id, int? userId = null)
		{
			try
			{
				await ExecuteNonQueryAsync("fn_product_master", cmd =>
				{
					cmd.Parameters.AddWithValue("p_action", "DELETE");
					cmd.Parameters.AddWithValue("p_product_id", id);
					cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
					cmd.Parameters.AddWithValue("p_user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
				});
				return DbResult.Ok();
			}
			catch (Exception ex)
			{
				return HandleException(ex);
			}
		}

		async Task IRepository<Product>.DeleteAsync(int id) => await DeleteAsync(id, null);

		public async Task<List<ProductWithDetails>> GetAllWithDetailsAsync()
		{
			return await ExecuteReaderAsync("sp_product_get_all_with_details",
			   cmd =>
			   {
				   cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
			   },
			   reader => new ProductWithDetails
			   {
				   ProductId = SafeGetInt(reader, "product_id"),
				   Type = SafeGetString(reader, "product_type"),
				   Name = SafeGetString(reader, "product_name"),
				   ShortName = SafeGetString(reader, "short_name"),
				   MRP = reader["mrp"] != DBNull.Value ? Convert.ToDecimal(reader["mrp"]) : (decimal?)null,
				   SellingPrice = reader["selling_price"] != DBNull.Value ? Convert.ToDecimal(reader["selling_price"]) : (decimal?)null,
				   SalesDescription = SafeGetString(reader, "sales_description"),
				   CostPrice = reader["cost_price"] != DBNull.Value ? Convert.ToDecimal(reader["cost_price"]) : (decimal?)null,
				   PurchaseDescription = SafeGetString(reader, "purchase_description"),
				   BaseUnit = SafeGetString(reader, "base_unit"),
				   PackingUnit = SafeGetString(reader, "packing_unit"),
				   ProductGroupName = SafeGetString(reader, "product_group_name"),
				   TaxGroupName = SafeGetString(reader, "tax_group_name"),
				   HSNCode = SafeGetString(reader, "hsn_code"),
				   SACCode = SafeGetString(reader, "sac_code"),
				   TaxPreferenceId = reader["tax_preference_id"] != DBNull.Value ? SafeGetInt(reader, "tax_preference_id") : (int?)null,
				   TaxGroupId = reader["tax_group_id"] != DBNull.Value ? SafeGetInt(reader, "tax_group_id") : (int?)null,
				   MinQuantity = reader["min_quantity"] != DBNull.Value ? Convert.ToDecimal(reader["min_quantity"]) : (decimal?)null,
				   MaxQuantity = reader["max_quantity"] != DBNull.Value ? Convert.ToDecimal(reader["max_quantity"]) : (decimal?)null,
				   ReOrderQuantity = reader["re_order_quantity"] != DBNull.Value ? Convert.ToDecimal(reader["re_order_quantity"]) : null,
				   CGSTPer = reader["cgst_per"] != DBNull.Value ? Convert.ToDecimal(reader["cgst_per"]) : (decimal?)null,
				   SGSTPer = reader["sgst_per"] != DBNull.Value ? Convert.ToDecimal(reader["sgst_per"]) : (decimal?)null,
				   UnitId = reader["unit_id"] != DBNull.Value ? SafeGetInt(reader, "unit_id") : (int?)null,
				   ProductPackingUnit = reader["product_packing_unit"] != DBNull.Value ? SafeGetInt(reader, "product_packing_unit") : (int?)null
			   });
		}
	}
}
