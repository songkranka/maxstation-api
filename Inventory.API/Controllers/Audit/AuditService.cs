using Inventory.API.Domain.Repositories;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Controllers.Audit
{
    public class AuditService : IAuditService
    {
        private IAuditRepository _repo = null;
        private IUnitOfWork _unitOfWork = null;
        public AuditService(IAuditRepository pRepo, IUnitOfWork pUnitOfwork)
        {
            _repo = pRepo;
            _unitOfWork = pUnitOfwork;

        }
        public async Task<ModelAudit> SaveData(ModelAudit pInput)
        {
            ModelAudit result = await _repo.SaveData(pInput);
            await _unitOfWork.CompleteAsync();
            return result;

        }

        public async Task<InvAuditHd> UpdateStatus(InvAuditHd pInput)
        {
            InvAuditHd result = _repo.UpdateStatus(pInput);
            await _unitOfWork.CompleteAsync();
            return result;
        }

        public async Task<ModelAuditResult> GetArrayAudit(ModelAuditParam pInput)
        {
            return await _repo.GetArrayAudit(pInput);
        }

        public async Task<ModelAudit> GetAudit(string pStrGuid)
        {
            return await _repo.GetAudit(pStrGuid);
        }

        public async Task<int> GetAuditCount(string compCode,string brnCode, int pIntAuditYear)
        {
            return await _repo.GetAuditCount(compCode,brnCode, pIntAuditYear);
        }

        public async Task<ModelAuditProduct> GetAuditProduct(ModelAuditProductParam pInput)
        {
            return await _repo.GetAuditProduct(pInput);
        }

        public async Task<MasEmployee> GetEmployee(string pStrEmpCode)
        {
            return await _repo.GetEmployee(pStrEmpCode);
        }


        public string GetHello()
        {
            return "Hello Wordl";
        }

        public async Task<MasProduct[]> GetArrayProduct(ModelAuditProductParam pInput)
        {
            return await _repo.GetArrayProduct(pInput);
        }
    }
}
