using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrder> ListAsync(PurchaseOrderHdQuery query);
        Task<PurchaseOrder> DetailListAsync(PurchaseOrderHdQuery query);
    }
}
