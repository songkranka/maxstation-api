using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Services;
using MasterData.API.Services;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;
        private readonly IMapper _mapper;
        private static readonly ILog _log = LogManager.GetLogger(typeof(MenuController));
        public MenuController(
            IMenuService menuService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _menuService = menuService;
            _mapper = mapper;

        }

        /// <summary>
        /// existing branchcode.
        /// </summary>
        /// <returns>menues.</returns>
        [HttpGet("FindByCompCodeAndBranchCode/{compCode}/{brnCode}")]
        public async Task<IActionResult> FindByIdAsync(string compCode, string brnCode)
        {
            ResponseData<Menus> response = new ResponseData<Menus>();

            try
            {
                response.Data = await _menuService.FindByBranchCodeAsync(compCode, brnCode);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }

        }
        [HttpGet("GetPositionRole/{pStrEmpCode}/{pStrRouteUrl}")]
        public async Task<IActionResult> GetPositionRole(string pStrEmpCode, string pStrRouteUrl)
        {
            return await DefaultService.DoActionAsync("GetPositionRole", async () => await _menuService.GetPositionRole(pStrEmpCode, pStrRouteUrl), _log);
        }
    }
}
