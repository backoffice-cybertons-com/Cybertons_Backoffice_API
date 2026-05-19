using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Classes
{
    public class DeliveryChallanRepository : BaseDAL, IDeliveryChallanRepository
    {
        public DeliveryChallanRepository(IConfiguration configuration, ITenantProvider tenantProvider)
            : base(configuration, tenantProvider)
        {
        }

        private static void AddQuantityParameter(NpgsqlCommand cmd, string name, decimal? value)
        {
            var parameter = cmd.Parameters.Add(name, NpgsqlTypes.NpgsqlDbType.Numeric);
            parameter.Precision = 18;
            parameter.Scale = 4;
            parameter.Value = (object?)value ?? DBNull.Value;
        }

        public async Task<List<DeliveryChallan>> GetAllAsync(int companyId)
        {
            return await ExecuteReaderAsync("public.fn_delivery_challan_master", cmd =>
            {
                cmd.Parameters.AddWithValue("p_action", "GETALL");
                cmd.Parameters.AddWithValue("p_company_id", companyId);
            }, MapDeliveryChallan);
        }

        public async Task<DeliveryChallan?> GetByIdAsync(int companyId, int deliveryChallanId)
        {
            return await ExecuteSingleReaderAsync("public.fn_delivery_challan_master", cmd =>
            {
                cmd.Parameters.AddWithValue("p_action", "GETBYID");
                cmd.Parameters.AddWithValue("p_delivery_challan_id", deliveryChallanId);
                cmd.Parameters.AddWithValue("p_company_id", companyId);
            }, MapDeliveryChallan);
        }

        public async Task<DbResult> InsertAsync(DeliveryChallan entity)
        {
            try
            {
                var result = await ExecuteSingleReaderAsync("public.fn_delivery_challan_master", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "INSERT");
                    AddDeliveryChallanParameters(cmd, entity);
                }, MapDeliveryChallan);
                return DbResult.Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<DbResult> UpdateAsync(DeliveryChallan entity)
        {
            try
            {
                var result = await ExecuteSingleReaderAsync("public.fn_delivery_challan_master", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "UPDATE");
                    AddDeliveryChallanParameters(cmd, entity);
                }, MapDeliveryChallan);
                return DbResult.Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<DbResult> DeleteAsync(int companyId, int deliveryChallanId)
        {
            try
            {
                await ExecuteNonQueryAsync("public.fn_delivery_challan_master", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "DELETE");
                    cmd.Parameters.AddWithValue("p_delivery_challan_id", deliveryChallanId);
                    cmd.Parameters.AddWithValue("p_company_id", companyId);
                });
                return DbResult.Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<List<DeliveryChallanItem>> GetItemsAsync(int companyId, int deliveryChallanId)
        {
            return await ExecuteReaderAsync("public.fn_delivery_challan_items", cmd =>
            {
                cmd.Parameters.AddWithValue("p_action", "GETALL");
                cmd.Parameters.AddWithValue("p_delivery_challan_id", deliveryChallanId);
                cmd.Parameters.AddWithValue("p_company_id", companyId);
            }, MapDeliveryChallanItem);
        }

        public async Task<DbResult> InsertItemAsync(DeliveryChallanItem entity)
        {
            try
            {
                var result = await ExecuteSingleReaderAsync("public.fn_delivery_challan_items", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "INSERT");
                    AddDeliveryChallanItemParameters(cmd, entity);
                }, MapDeliveryChallanItem);
                return DbResult.Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<DbResult> UpdateItemAsync(DeliveryChallanItem entity)
        {
            try
            {
                var result = await ExecuteSingleReaderAsync("public.fn_delivery_challan_items", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "UPDATE");
                    AddDeliveryChallanItemParameters(cmd, entity);
                }, MapDeliveryChallanItem);
                return DbResult.Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<DbResult> DeleteItemAsync(int companyId, int itemId)
        {
            try
            {
                await ExecuteNonQueryAsync("public.fn_delivery_challan_items", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "DELETE");
                    cmd.Parameters.AddWithValue("p_item_id", itemId);
                    cmd.Parameters.AddWithValue("p_company_id", companyId);
                });
                return DbResult.Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<List<DeliveryChallanItem>> GetAllWithItemsAsync(int companyId, int? deliveryChallanId = null)
        {
             return await ExecuteReaderAsync("public.fn_delivery_challan_get_with_items", cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", companyId);
                cmd.Parameters.AddWithValue("p_delivery_challan_id", (object?)deliveryChallanId ?? DBNull.Value);
            }, MapDeliveryChallanItem); 
        }

        public async Task<List<dynamic>> GetAllDetailedAsync(int companyId)
        {
            var results = await ExecuteReaderAsync("public.fn_delivery_challan_get_with_items", cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", companyId);
                cmd.Parameters.AddWithValue("p_delivery_challan_id", DBNull.Value);
            }, reader => {
                var item = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    item.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                }
                return (dynamic)item;
            });

            // Filter out empty rows where delivery_challan_id is null
            return results.Where(r => {
                var dict = (IDictionary<string, object>)r;
                return dict.ContainsKey("delivery_challan_id") && dict["delivery_challan_id"] != null;
            }).ToList();
        }

        private DeliveryChallan MapDeliveryChallan(NpgsqlDataReader reader)
        {
            return new DeliveryChallan
            {
                DeliveryChallanId = SafeGetInt(reader, "delivery_challan_id"),
                CompanyId = SafeGetInt(reader, "company_id"),
                CustomerId = SafeGetInt(reader, "customer_id"),
                LocationId = reader.IsDBNull(reader.GetOrdinal("location_id")) ? (int?)null : SafeGetInt(reader, "location_id"),
                ChallanNumber = SafeGetString(reader, "challan_number"),
                ReferenceNo = SafeGetString(reader, "reference_no"),
                ChallanDate = reader.GetDateTime(reader.GetOrdinal("challan_date")),
                ChallanType = SafeGetString(reader, "challan_type"),
                CustomerNotes = SafeGetString(reader, "customer_notes"),
                Terms = SafeGetString(reader, "terms"),
                WarehouseLocation = SafeGetString(reader, "warehouse_location")
            };
        }

        private DeliveryChallanItem MapDeliveryChallanItem(NpgsqlDataReader reader)
        {
            return new DeliveryChallanItem
            {
                ItemId = SafeGetInt(reader, "item_id"),
                CompanyId = SafeGetInt(reader, "company_id"),
                DeliveryChallanId = reader.IsDBNull(reader.GetOrdinal("delivery_challan_id")) ? (int?)null : SafeGetInt(reader, "delivery_challan_id"),
                ProductId = reader.IsDBNull(reader.GetOrdinal("product_id")) ? (int?)null : SafeGetInt(reader, "product_id"),
                ItemDetails = SafeGetString(reader, "item_details"),
                Quantity = reader.GetDecimal(reader.GetOrdinal("quantity")),
                Rate = reader.GetDecimal(reader.GetOrdinal("rate")),
                TaxId = reader.IsDBNull(reader.GetOrdinal("tax_id")) ? (int?)null : SafeGetInt(reader, "tax_id"),
                Amount = reader.GetDecimal(reader.GetOrdinal("amount")),
                DiscountPct = reader.IsDBNull(reader.GetOrdinal("discount_pct")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("discount_pct")),
                Adjustment = reader.IsDBNull(reader.GetOrdinal("adjustment")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("adjustment"))
            };
        }

        private void AddDeliveryChallanParameters(NpgsqlCommand cmd, DeliveryChallan entity)
        {
            cmd.Parameters.Add("p_delivery_challan_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.DeliveryChallanId == null || entity.DeliveryChallanId == 0 ? DBNull.Value : entity.DeliveryChallanId;
            cmd.Parameters.Add("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = entity.CompanyId;
            cmd.Parameters.Add("p_customer_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = entity.CustomerId;
            cmd.Parameters.Add("p_location_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.LocationId ?? DBNull.Value;
            cmd.Parameters.Add("p_challan_number", NpgsqlTypes.NpgsqlDbType.Varchar).Value = entity.ChallanNumber;
            cmd.Parameters.Add("p_reference_no", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.ReferenceNo ?? DBNull.Value;
            cmd.Parameters.Add("p_challan_date", NpgsqlTypes.NpgsqlDbType.Date).Value = entity.ChallanDate;
            cmd.Parameters.Add("p_challan_type", NpgsqlTypes.NpgsqlDbType.Varchar).Value = entity.ChallanType;
            cmd.Parameters.Add("p_customer_notes", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)entity.CustomerNotes ?? DBNull.Value;
            cmd.Parameters.Add("p_terms", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)entity.Terms ?? DBNull.Value;
            cmd.Parameters.Add("p_warehouse_location", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.WarehouseLocation ?? DBNull.Value;
        }

        private void AddDeliveryChallanItemParameters(NpgsqlCommand cmd, DeliveryChallanItem entity)
        {
            cmd.Parameters.Add("p_item_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.ItemId == null || entity.ItemId == 0 ? DBNull.Value : entity.ItemId;
            cmd.Parameters.Add("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = entity.CompanyId;
            cmd.Parameters.Add("p_delivery_challan_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.DeliveryChallanId ?? DBNull.Value;
            cmd.Parameters.Add("p_product_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.ProductId ?? DBNull.Value;
            cmd.Parameters.Add("p_item_details", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)entity.ItemDetails ?? DBNull.Value;
            AddQuantityParameter(cmd, "p_quantity", entity.Quantity);
            cmd.Parameters.Add("p_rate", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.Rate;
            cmd.Parameters.Add("p_tax_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.TaxId ?? DBNull.Value;
            cmd.Parameters.Add("p_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.Amount;
            cmd.Parameters.Add("p_discount_pct", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.DiscountPct ?? DBNull.Value;
            cmd.Parameters.Add("p_adjustment", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.Adjustment ?? DBNull.Value;
        }
    }
}
