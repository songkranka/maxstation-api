using MaxStation.Entities.Models;
using System.Threading.Tasks;

namespace Inventory.API.Controllers.Audit
{
    public interface IAuditRepository
    {
        Task<ModelAuditResult> GetArrayAudit(ModelAuditParam pInput);
        Task<ModelAudit> GetAudit(string pStrGuid);
        Task<int> GetAuditCount(string compCode,string brnCode,int pIntAuditYear);
        Task<ModelAudit> SaveData(ModelAudit pInput);
        string SayHello();
        InvAuditHd UpdateStatus(InvAuditHd pInput);
        Task<ModelAuditProduct> GetAuditProduct(ModelAuditProductParam pInput);
        Task<MasProduct[]> GetArrayProduct(ModelAuditProductParam pInput);
        Task<MasEmployee> GetEmployee(string pStrEmpCode);

    }
}