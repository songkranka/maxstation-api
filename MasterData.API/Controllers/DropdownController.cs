using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Dropdowns;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Services;
using MasterData.API.Resources.CustomerCar;
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
    public class DropdownController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        private readonly IMapper _mapper;
        private readonly ICustomerCarService _customerCarService;
        private readonly ICostCenterService _costCenterService;
        private readonly IDropdownService _dropdownService;
        private readonly IBranchService _branchService;

        public DropdownController(
            IMapper mapper,
            ICustomerCarService customerCarService,
            ICostCenterService costCenterService,
            IDropdownService dropdownService,
            IBranchService branchService,
            PTMaxstationContext context) : base(context)
        {
            _customerCarService = customerCarService;
            _costCenterService = costCenterService;
            _dropdownService = dropdownService;
            _branchService = branchService;
            _mapper = mapper;
        }

        /// <summary>
        /// Lists all existing customercar.
        /// </summary>
        /// <returns>List of customercar.</returns>
        [HttpPost("GetCustomerCars")]
        [ProducesResponseType(typeof(CustomerCar), 200)]
        public QueryResult<CustomerCar> GetCustomerCars([FromBody] Resources.CustomerCar.CustomerCarQuery query)
        {
            return _customerCarService.CustomerCarDropdownList(query);
        }

        [HttpPost("GetCostCenters")]
        public IActionResult GetCostCenters([FromBody] CostCenterRequest req)
        {
            ResponseData<List<MasCostCenter>> response = new ResponseData<List<MasCostCenter>>();
            try
            {
                response.Data = _costCenterService.GetCostCenterList(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }

        [HttpPost("GetLicensePlate")]
        public IActionResult GetLicensePlates([FromBody] LicensePlateRequest req)
        {
            ResponseData<List<MasCompanyCar>> response = new ResponseData<List<MasCompanyCar>>();
            try
            {
                response.Data = _dropdownService.GetCompanyCarLicenseList(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }

        [HttpPost("GetBranch")]
        public IActionResult GetBranch([FromBody] BranchDropdownRequest req)
        {
            ResponseData<List<MasBranch>> response = new ResponseData<List<MasBranch>>();
            try
            {
                response.Data = _dropdownService.GetBranchList(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }

        [HttpPost("GetBranchEmployeeAuth")]
        public async Task<IActionResult> GetBranchEmployeeAuth([FromBody] BranchEmployeeAuthRequest req)
        {
            ResponseData<List<MasBranch>> response = new ResponseData<List<MasBranch>>();
            try
            {
                response.Data = await _branchService.GetBranchEmployeeAuth(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }
    }
}
