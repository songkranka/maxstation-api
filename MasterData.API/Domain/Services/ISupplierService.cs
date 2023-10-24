
using MasterData.API.Domain.Models.Request;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface ISupplierService
    {
        Task<ModelGetSupplierListResult> GetSupplierList(ModelGetSupplierListParam param);
        Task<ModelSupplier> GetSupplier(string pStrGuid);
        Task<bool> CheckDuplicateCode(string pStrSupCode);
        Task<MasSupplier> UpdateStatus(MasSupplier param);
        Task<ModelSupplier> InsertSupplier(ModelSupplier param);
        Task<ModelSupplier> UpdateSupplier(ModelSupplier param);
    }
}
