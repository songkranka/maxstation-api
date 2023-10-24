using Inventory.API.Domain.Models;
using Inventory.API.Resources.SupplyRequest;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services
{
    public interface ISupplyRequestService
    {
        Task<List<SupplyRequest>> GetRequestDataByRequest(SupplyRequestByRequestQuery request);
        //Task SaveExcelFile(List<SupplyRequest> supplyRequests, FileInfo file);
        List<SupplyRequest> LoadExcelFile(IFormFile file);
    }
}
