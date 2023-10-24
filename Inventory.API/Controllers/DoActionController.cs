using Inventory.API.Services;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Controllers.Audit
{
    public class DoActionController<TypeService> : ControllerBase
    {
        private static ILog _log = null;
        protected TypeService _service = default(TypeService);
        public DoActionController(TypeService pService)
        {
            _service = pService;
            _log = LogManager.GetLogger(GetControllerType());

        }

        protected virtual Type GetControllerType()
        {
            return typeof(DoActionController<TypeService>);
        }

        public async Task<IActionResult> DoAction<T>(string pStrFunctionName, Func<Task<T>> pFunc)
        {
            return await DefaultService.DoActionAsync(pStrFunctionName, pFunc, _log);
        }        

    }
}
