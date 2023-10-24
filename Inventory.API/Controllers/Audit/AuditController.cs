using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Controllers.Audit
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : DoActionController<IAuditService>
    {
        public AuditController(IAuditService pService) : base(pService)
        {
        }

        protected override Type GetControllerType()
        {
            return typeof(AuditController);
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData(ModelAudit pInput)
        {
            return await DoAction("SaveData", async()=> await _service.SaveData(pInput));
        }
        
        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(InvAuditHd pInput)
        {
            return await DoAction("UpdateStatus", async () => await _service.UpdateStatus(pInput));
        }

        [HttpPost("GetArrayAudit")]
        public async Task<IActionResult> GetArrayAudit(ModelAuditParam pInput)
        {
            return await DoAction("GetArrayAudit", async () => await _service.GetArrayAudit(pInput));
        }

        [HttpGet("GetAudit/{pStrGuid}")]
        public async Task<IActionResult> GetAudit(string pStrGuid)
        {
            return await DoAction("GetAudit", async () => await _service.GetAudit(pStrGuid));
        }

        [HttpGet("GetAuditCount/{compCode}/{brnCode}/{pIntAuditYear}")]
        public async Task<IActionResult> GetAuditCount(string compCode,string brnCode, int pIntAuditYear)
        {
            return await DoAction("GetAuditCount", async () => await _service.GetAuditCount(compCode,brnCode, pIntAuditYear));
        }

        [HttpPost("GetAuditProduct")]
        public async Task<IActionResult> GetAuditProduct(ModelAuditProductParam pInput)
        {
            return await DoAction("GetAuditProduct", async () => await _service.GetAuditProduct(pInput));
        }

        [HttpPost("GetArrayProduct")]
        public async Task<IActionResult> GetArrayProduct(ModelAuditProductParam pInput)
        {
            return await DoAction("GetArrayProduct", async () => await _service.GetArrayProduct(pInput));
        }
        //public async Task<MasEmployee> GetEmployee(string pStrEmpCode)
        [HttpGet("GetEmployee/{pStrEmpCode}")]
        public async Task<IActionResult> GetEmployee(string pStrEmpCode)
        {
            return await DoAction("GetEmployee", async () => await _service.GetEmployee(pStrEmpCode));
        }
    }
}
