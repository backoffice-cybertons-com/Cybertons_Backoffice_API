using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
  using SoftBook.Domain.Data;
  using SoftBook.Domain.Entities;
  using SoftBook.Domain.RepositoryInterfaces;

  namespace SoftBook.Infrastructure.Repositories.Classes
  {
    public class ProductRepository : BaseDAL, IProductRepository
    {
      public ProductRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

      public async Task<List<Product>> GetAllAsync()
      {
        return await ExecuteReaderAsync("sp_Product_Master", cmd =>
        {
          cmd.Parameters.AddWithValue("@Action", "SELECT");
        }, reader => new Product
        {
          ProductId = Convert.ToInt32(reader["ProductId"]),
          Type = reader["Type"]?.ToString(),
          Name = reader["Name"]?.ToString(),
          SellingPrice = reader["SellingPrice"] != DBNull.Value ? Convert.ToDecimal(reader["SellingPrice"]) : null,
          SalesAccount = reader["SalesAccount"]?.ToString(),
          SalesDescription = reader["SalesDescription"]?.ToString(),
          CostPrice = reader["CostPrice"] != DBNull.Value ? Convert.ToDecimal(reader["CostPrice"]) : null,
          PurchaseAccount = reader["PurchaseAccount"]?.ToString(),
          PurchaseDescription = reader["PurchaseDescription"]?.ToString(),
          PreferredVendorId = reader["PreferredVendorId"] != DBNull.Value ? Convert.ToInt32(reader["PreferredVendorId"]) : null,
          Sellable = reader["Sellable"] != DBNull.Value && Convert.ToBoolean(reader["Sellable"]),
          Purchasable = reader["Purchasable"] != DBNull.Value && Convert.ToBoolean(reader["Purchasable"]),
          UnitId = reader["UnitId"] != DBNull.Value ? Convert.ToInt32(reader["UnitId"]) : null,
          HSNId = reader["HSNId"] != DBNull.Value ? Convert.ToInt32(reader["HSNId"]) : null,
          SACId = reader["SACId"] != DBNull.Value ? Convert.ToInt32(reader["SACId"]) : null,
          TaxPreferenceId = reader["TaxPreferenceId"] != DBNull.Value ? Convert.ToInt32(reader["TaxPreferenceId"]) : null,
          TaxGroupId = reader["TaxGroupId"] != DBNull.Value ? Convert.ToInt32(reader["TaxGroupId"]) : null
        });
      }

      public async Task<Product?> GetByIdAsync(int id)
      {
        return await ExecuteSingleReaderAsync("sp_Product_Master",
            cmd =>
            {
              cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
              cmd.Parameters.AddWithValue("@ProductId", id);
            },
            reader => new Product
            {
              ProductId = Convert.ToInt32(reader["ProductId"]),
              Type = reader["Type"]?.ToString(),
              Name = reader["Name"]?.ToString(),
              SellingPrice = reader["SellingPrice"] != DBNull.Value ? Convert.ToDecimal(reader["SellingPrice"]) : null,
              SalesAccount = reader["SalesAccount"]?.ToString(),
              SalesDescription = reader["SalesDescription"]?.ToString(),
              CostPrice = reader["CostPrice"] != DBNull.Value ? Convert.ToDecimal(reader["CostPrice"]) : null,
              PurchaseAccount = reader["PurchaseAccount"]?.ToString(),
              PurchaseDescription = reader["PurchaseDescription"]?.ToString(),
              PreferredVendorId = reader["PreferredVendorId"] != DBNull.Value ? Convert.ToInt32(reader["PreferredVendorId"]) : null,
              Sellable = reader["Sellable"] != DBNull.Value && Convert.ToBoolean(reader["Sellable"]),
              Purchasable = reader["Purchasable"] != DBNull.Value && Convert.ToBoolean(reader["Purchasable"]),

              UnitId = reader["UnitId"] != DBNull.Value ? Convert.ToInt32(reader["UnitId"]) : null,
              HSNId = reader["HSNId"] != DBNull.Value ? Convert.ToInt32(reader["HSNId"]) : null,
              SACId = reader["SACId"] != DBNull.Value ? Convert.ToInt32(reader["SACId"]) : null,
              TaxPreferenceId = reader["TaxPreferenceId"] != DBNull.Value ? Convert.ToInt32(reader["TaxPreferenceId"]) : null,
              TaxGroupId = reader["TaxGroupId"] != DBNull.Value ? Convert.ToInt32(reader["TaxGroupId"]) : null
            });
      }

      public async Task InsertAsync(Product product)
      {
        await ExecuteNonQueryAsync("sp_Product_Master", cmd =>
        {
          cmd.Parameters.AddWithValue("@Action", "INSERT");
          cmd.Parameters.AddWithValue("@Type", product.Type ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@Name", product.Name ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@SellingPrice", product.SellingPrice ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@SalesAccount", product.SalesAccount ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@SalesDescription", product.SalesDescription ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@CostPrice", product.CostPrice ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@PurchaseAccount", product.PurchaseAccount ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@PurchaseDescription", product.PurchaseDescription ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@PreferredVendorId", product.PreferredVendorId ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@Sellable", product.Sellable);
          cmd.Parameters.AddWithValue("@Purchasable", product.Purchasable);
          cmd.Parameters.AddWithValue("@UnitId", product.UnitId ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@HSNId", product.HSNId ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@SACId", product.SACId ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@TaxPreferenceId", product.TaxPreferenceId ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@TaxGroupId", product.TaxGroupId ?? (object)DBNull.Value);
        });
      }

      public async Task UpdateAsync(Product product)
      {
        await ExecuteNonQueryAsync("sp_Product_Master", cmd =>
        {
          cmd.Parameters.AddWithValue("@Action", "UPDATE");
          cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
          cmd.Parameters.AddWithValue("@Type", product.Type ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@Name", product.Name ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@SellingPrice", product.SellingPrice ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@SalesAccount", product.SalesAccount ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@SalesDescription", product.SalesDescription ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@CostPrice", product.CostPrice ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@PurchaseAccount", product.PurchaseAccount ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@PurchaseDescription", product.PurchaseDescription ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@PreferredVendorId", product.PreferredVendorId ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@Sellable", product.Sellable);
          cmd.Parameters.AddWithValue("@Purchasable", product.Purchasable);
          cmd.Parameters.AddWithValue("@UnitId", product.UnitId ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@HSNId", product.HSNId ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@SACId", product.SACId ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@TaxPreferenceId", product.TaxPreferenceId ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("@TaxGroupId", product.TaxGroupId ?? (object)DBNull.Value);
        });
      }

      public async Task DeleteAsync(int id)
      {
        await ExecuteNonQueryAsync("sp_Product_Master", cmd =>
        {
          cmd.Parameters.AddWithValue("@Action", "DELETE");
          cmd.Parameters.AddWithValue("@ProductId", id);
        });
      }

    public async Task<List<ProductWithDetails>> GetAllWithDetailsAsync()
    {
      return await ExecuteReaderAsync("sp_Product_Master",
         cmd => cmd.Parameters.AddWithValue("@Action", "SELECT_WITH_DETAILS"),
         reader => new ProductWithDetails
         {
           ProductId = Convert.ToInt32(reader["ProductId"]),
           Type = reader["Type"]?.ToString(),
           Name = reader["Name"]?.ToString(),
           SellingPrice = reader["SalesRate"] != DBNull.Value ? Convert.ToDecimal(reader["SalesRate"]) : null,
           SalesDescription = reader["SalesDescription"]?.ToString(),
           CostPrice = reader["PurchaseRate"] != DBNull.Value ? Convert.ToDecimal(reader["PurchaseRate"]) : null,
           PurchaseDescription = reader["PurchaseDescription"]?.ToString(),
           UnitName = reader["UsageUnit"]?.ToString(),
           HSNCode = reader["HSNCode"]?.ToString(),
           SACCode = reader["SACCode"]?.ToString(),
           TaxPreferenceId = reader["TaxPreferenceId"] != DBNull.Value ? Convert.ToInt32(reader["TaxPreferenceId"]) : null,
           TaxGroupId = reader["TaxGroupId"] != DBNull.Value ? Convert.ToInt32(reader["TaxGroupId"]) : null
         });
    }
  }
  }
