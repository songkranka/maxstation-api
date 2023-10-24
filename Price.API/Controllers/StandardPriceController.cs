using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Price.API.Domain.Models;
using Price.API.Domain.Services;
using Price.API.Services;
using System;
using System.Threading.Tasks;

namespace Price.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StandardPriceController : Controller
    {
        #region - Variable -
        private IStandardPriceService _svStandardPrice = null;
        private ILog _log = null;
        #endregion
        #region - Controller -
        [HttpPost("GetArrayHeader")]
        public async Task<IActionResult> GetArrayHeader(ModelStandardPriceParam param)
        {
            return await doAction("GetArrayHeader", async () => await _svStandardPrice.GetArrayHeader(param));
        }
        [HttpPost("SaveStandardPrice")]
        public async Task<IActionResult> SaveStandardPrice(ModelStandardPrice param)
        {
            return await doAction("SaveStandardPrice", async () => await _svStandardPrice.SaveStandardPrice(param));
        }
        [HttpGet("GetStandardPrice/{pStrGuid}")]
        public async Task<IActionResult> GetStandardPrice(string pStrGuid)
        {
            return await doAction("GetStandardPrice", async () => await _svStandardPrice.GetStandardPrice(pStrGuid));
        }
        [HttpGet("GetArrayBranch/{pStrComCode}")]
        public async Task<IActionResult> GetArrayBranch(string pStrComCode)
        {
            return await doAction("GetArrayBranch", async () => await _svStandardPrice.GetArrayBranch(pStrComCode));
        }
        [HttpPost("GetArrayProduct")]
        public async Task<IActionResult> GetArrayProduct(MasProductPrice param)
        {
            return await doAction("GetArrayProduct", async () => await _svStandardPrice.GetArrayProduct(param));
        }
        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(OilStandardPriceHd param)
        {
            return await doAction("UpdateStatus", async () => await _svStandardPrice.UpdateStatus(param));
        }
        [HttpGet("GetArrayStandardPriceDetail/{pStrCompCode}/{pStrBrnCode}")]
        public async Task<IActionResult> GetArrayStandardPriceDetail(string pStrCompCode, string pStrBrnCode)
        {
            return await doAction("GetArrayStandardPriceDetail", async () => await _svStandardPrice.GetArrayStandardPriceDetail(pStrCompCode , pStrBrnCode));
        }
        [HttpGet("GetUnApproveDocument/{pStrCompCode}")]
        public async Task<IActionResult> GetUnApproveDocument(string pStrCompCode)
        {
            return await doAction("GetUnApproveDocument", async () => await _svStandardPrice.GetUnApproveDocument(pStrCompCode));
        }
        #endregion
        #region - Constructor -
        public StandardPriceController(IStandardPriceService pService)
        {
            _svStandardPrice = pService;
            _log = LogManager.GetLogger(typeof(StandardPriceController));
        }

        #endregion
        #region - Function -
        private async Task<IActionResult> doAction<T>(string pStrFunctionName, Func<Task<T>> pFunc)
        {
            return await DefaultService.DoActionAsync(pStrFunctionName, pFunc, _log);            
        }
        
        #endregion
    }
}