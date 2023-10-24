using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Repositories;
using Inventory.API.Domain.Services;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IUnitOfWork _unitOfWork;
        public PurchaseOrderService(
            IPurchaseOrderRepository purchaseOrderRepository,
            IUnitOfWork unitOfWork)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PurchaseOrder> ListAsync(PurchaseOrderHdQuery query)
        {
            return await _purchaseOrderRepository.ListAsync(query);
        }

        public async Task<PurchaseOrder> DetailListAsync(PurchaseOrderHdQuery query)
        {
            return await _purchaseOrderRepository.DetailListAsync(query);
        }
    }
}
