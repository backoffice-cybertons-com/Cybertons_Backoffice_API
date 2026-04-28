using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class DeliveryChallanService : IDeliveryChallanService
    {
        private readonly IDeliveryChallanRepository _deliveryChallanRepository;

        public DeliveryChallanService(IDeliveryChallanRepository deliveryChallanRepository)
        {
            _deliveryChallanRepository = deliveryChallanRepository;
        }

        public async Task<List<DeliveryChallan>> GetAllAsync(int companyId)
        {
            return await _deliveryChallanRepository.GetAllAsync(companyId);
        }

        public async Task<List<dynamic>> GetAllDetailedAsync(int companyId)
        {
            return await _deliveryChallanRepository.GetAllDetailedAsync(companyId);
        }

        public async Task<DeliveryChallanViewModel?> GetByIdAsync(int companyId, int deliveryChallanId)
        {
            var header = await _deliveryChallanRepository.GetByIdAsync(companyId, deliveryChallanId);
            if (header == null) return null;

            var items = await _deliveryChallanRepository.GetItemsAsync(companyId, deliveryChallanId);

            return new DeliveryChallanViewModel
            {
                Header = header,
                Items = items
            };
        }

        public async Task<DbResult> InsertCompositeAsync(DeliveryChallanViewModel viewModel)
        {
            var result = await _deliveryChallanRepository.InsertAsync(viewModel.Header);
            if (!result.Success) return result;

            var savedHeader = (DeliveryChallan)result.Data!;
            int deliveryChallanId = savedHeader.DeliveryChallanId;

            foreach (var item in viewModel.Items)
            {
                item.DeliveryChallanId = deliveryChallanId;
                item.CompanyId = viewModel.Header.CompanyId;
                await _deliveryChallanRepository.InsertItemAsync(item);
            }

            return result;
        }

        public async Task<DbResult> UpdateCompositeAsync(DeliveryChallanViewModel viewModel)
        {
            var result = await _deliveryChallanRepository.UpdateAsync(viewModel.Header);
            if (!result.Success) return result;

            int deliveryChallanId = viewModel.Header.DeliveryChallanId;

            // Handle items: this is a simple implementation that updates existing items or inserts new ones.
            // Depending on requirements, we might need to delete items not present in the viewModel.Items list.
            foreach (var item in viewModel.Items)
            {
                item.DeliveryChallanId = deliveryChallanId;
                item.CompanyId = viewModel.Header.CompanyId;

                if (item.ItemId == 0)
                {
                    await _deliveryChallanRepository.InsertItemAsync(item);
                }
                else
                {
                    await _deliveryChallanRepository.UpdateItemAsync(item);
                }
            }

            return result;
        }

        public async Task<DbResult> DeleteAsync(int companyId, int deliveryChallanId)
        {
            var items = await _deliveryChallanRepository.GetItemsAsync(companyId, deliveryChallanId);
            if (items != null)
            {
                foreach (var item in items)
                {
                    await _deliveryChallanRepository.DeleteItemAsync(companyId, item.ItemId);
                }
            }
            return await _deliveryChallanRepository.DeleteAsync(companyId, deliveryChallanId);
        }
    }
}
