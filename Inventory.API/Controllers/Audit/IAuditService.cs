using MaxStation.Entities.Models;
using System.Threading.Tasks;

namespace Inventory.API.Controllers.Audit
{
    public interface IAuditService
    {
        Task<ModelAuditResult> GetArrayAudit(ModelAuditParam pInput);
        Task<ModelAudit> GetAudit(string pStrGuid);
        Task<int> GetAuditCount(string compCode,string brnCode,int pIntAuditYear);
        string GetHello();
        Task<ModelAudit> SaveData(ModelAudit pInput);
        Task<InvAuditHd> UpdateStatus(InvAuditHd pInput);
        Task<ModelAuditProduct> GetAuditProduct(ModelAuditProductParam pInput);
        Task<MasProduct[]> GetArrayProduct(ModelAuditProductParam pInput);
        Task<MasEmployee> GetEmployee(string pStrEmpCode);
    }
}