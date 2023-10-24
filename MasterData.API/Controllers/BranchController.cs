using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Services;
using MasterData.API.Resources;
using MasterData.API.Resources.Branch;
using MasterData.API.Services;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : BaseController
    {
        private readonly IBranchService _branchService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        private readonly IMapper _mapper;
        public BranchController(
            IBranchService branchService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)

        {
            _branchService = branchService;
            _mapper = mapper;
        }

        [HttpPost("GetBranchList")]
        public IActionResult GetBranchList([FromBody] BranchRequest req)
        {
            ResponseData<List<MasBranch>> response = new ResponseData<List<MasBranch>>();
            try
            {
                response.Data = _branchService.GetBranchList(req);
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

        [HttpPost("GetAuthBranchList")]
        public IActionResult GetAuthBranchList([FromBody] AuthBranchRequest req)
        {
            ResponseData<List<MasBranch>> response = new ResponseData<List<MasBranch>>();
            try
            {
                response.Data = _branchService.GetAuthBranchList(req);
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

        [HttpPost("List")]
        [ProducesResponseType(typeof(QueryResultResource<MasBranch>), 200)]
        public async Task<QueryResult<MasBranch>> List([FromBody] BranchQueryReqource pQuery)
        {
            var branchQuery = _mapper.Map<BranchQueryReqource, BranchQuery>(pQuery);
            var resource = await _branchService.List(branchQuery);
            return resource;
        }

        [HttpGet("GetCompanyDDL")]
        public IActionResult GetCompanyDDL()
        {
            ResponseData<List<MasCompany>> response = new ResponseData<List<MasCompany>>();
            try
            {
                response.Data = _branchService.getCompanyDDL();
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

        [HttpGet("GetCompany/{CompCode}")]
        public async Task<IActionResult> GetCompany(string CompCode)
        {
            ResponseData<MasCompany> response = new ResponseData<MasCompany>();
            try
            {
                response.Data = await _branchService.getCompany(CompCode);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error("GetCompany Error", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("GetBranchDetail/{BrnCode}")]
        public async Task<IActionResult> GetBranchDetail(string brnCode)
        {
            ResponseData<MasBranch> response = new ResponseData<MasBranch>();
            try
            {
                response.Data = await _branchService.getBranchDetail(brnCode);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                log.Info("GetBranchDetail Success");
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error("GetBranchDetail Error", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("GetBranchByGuid/{guid}")]
        public async Task<IActionResult> GetBranchDetail(Guid guid)
        {
            ResponseData<MasBranch> response = new ResponseData<MasBranch>();
            try
            {
                response.Data = await _branchService.GetBranchByGuid(guid);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error("GetBranchDetail Error", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("GetBranchByCompCodeAndBrnCode/{Compcode}/{BrnCode}")]
        public async Task<IActionResult> GetBranchByCompCodeAndBrnCode(string Compcode, string BrnCode)
        {
            ResponseData<MasBranch> response = new ResponseData<MasBranch>();
            try
            {
                response.Data = await _branchService.GetBranchDetailByCompCodeAndBrnCode(Compcode, BrnCode);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error("GetBranchDetail Error", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("GetBranchDropdown/{Compcode}/{BrnCode}")]
        public async Task<IActionResult> GetBranchDropdown(string Compcode, string BrnCode)
        {
            ResponseData<BranchResource> response = new ResponseData<BranchResource>();
            try
            {
                //response.Data = await _branchService.GetBranchDetailByCompCodeAndBrnCode(Compcode, BrnCode);
                var masBranchs = await _branchService.GetBranchDetailByCompCodeAndBrnCode(Compcode, BrnCode);
                var resource = _mapper.Map<MasBranch, BranchResource>(masBranchs);
                response.Data = resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error("GetBranchDetail Error", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("GetTankByBranch/{Compcode}/{BrnCode}")]
        public IActionResult GetTankByBranch(string Compcode, string BrnCode)
        {
            ResponseData<List<MasBranchTank>> response = new ResponseData<List<MasBranchTank>>();
            try
            {
                response.Data = _branchService.getTankByBranch(Compcode, BrnCode);
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

        [HttpGet("GetDispByBranch/{Compcode}/{BrnCode}")]
        public IActionResult GetDispByBranch(string Compcode, string BrnCode)
        {
            ResponseData<List<MasBranchDisp>> response = new ResponseData<List<MasBranchDisp>>();
            try
            {
                response.Data = _branchService.getDispByBranch(Compcode, BrnCode);
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

        [HttpGet("GetTaxByBranch/{Compcode}/{BrnCode}")]
        public IActionResult GetTaxByBranch(string Compcode, string BrnCode)
        {
            ResponseData<List<MasBranchTax>> response = new ResponseData<List<MasBranchTax>>();
            try
            {
                response.Data = _branchService.getTaxByBranch(Compcode, BrnCode);
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

        [HttpGet("GetAllBranchByCompCode{Compcode}")]
        public IActionResult GetAllBranchByCompCode(string Compcode)
        {
            ResponseData<List<MasBranch>> response = new ResponseData<List<MasBranch>>();
            try
            {
                response.Data = _branchService.GetAllBranchByCompCode(Compcode);
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

        [HttpPost("SaveBranch")]
        public async Task<IActionResult> SaveBranch([FromBody] SaveBranchRequest request)
        {
            ResponseData<MasBranch> response = new ResponseData<MasBranch>();

            try
            {
                var result = await _branchService.SaveBranchAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("SaveBranchTank")]
        public async Task<IActionResult> SaveBranchTank([FromBody] SaveBranchTankRequest request)
        {
            ResponseData<BranchTankResponse> response = new ResponseData<BranchTankResponse>();

            try
            {
                var result = await _branchService.SaveBranchTankAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("SaveBranchTax")]
        public async Task<IActionResult> SaveBranchTax([FromBody] SaveBranchTaxRequest request)
        {
            ResponseData<BranchTaxResponse> response = new ResponseData<BranchTaxResponse>();

            try
            {
                var result = await _branchService.SaveBranchTaxAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("GetMasBranchDISP")]
        [ProducesResponseType(typeof(QueryResultResource<MeterResponse>), 200)]
        public async Task<QueryResultResource<MeterResponse>> GetMasBranchDISP([FromBody] BranchMeterRequest req)
        {
            QueryResultResource<MeterResponse> response = new QueryResultResource<MeterResponse>();
            try
            {
                var queryResult = await _branchService.GetMasBranchDISP(req);

                response = _mapper.Map<QueryResult<MeterResponse>, QueryResultResource<MeterResponse>>(queryResult);
                response.IsSuccess = true;
                response.Message = "";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        [HttpPost("GetQueryOilPrice")]
        public async Task<string> GetQueryOilPrice([FromBody] BranchMeterRequest req)
        {
            var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var periodDateTime = new DateTime(docDate.Year, docDate.Month, docDate.Day, 0, 0, 0);
            var listOilPrice = new List<Domain.Models.Responses.Price.OilPrice>();

            var brnCode = req.BrnCode;
            var compCode = req.CompCode;
            var masPeriod = await context.MasBranchPeriods.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.PeriodNo == req.PeriodNo).AsNoTracking().FirstOrDefaultAsync();
            var periodStart = "";
            var periodFinish = "";
            if (masPeriod != null)
            {
                periodStart = masPeriod.TimeStart.Replace('.', ':');
                periodFinish = masPeriod.TimeFinish.Replace('.', ':');

                var periodHour = Convert.ToInt32(periodStart.Split(':')[0]);
                var periodMinute = Convert.ToInt32(periodStart.Split(':')[1]);
                periodDateTime = periodDateTime.AddHours(periodHour).AddMinutes(periodMinute);
            }
            //var sysDateMidNight = new DateTime(docDate.Year, docDate.Month, docDate.Day, 0, 0, 0).AddHours(23).AddMinutes(59).AddSeconds(59);
            var _curCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            string sysDateTime = periodDateTime.ToString("dd/MM/yyyy HH:mm:ss", _curCulture);
            string result = $@"select xpri.comp_code, xpri.brn_code, xpri.pd_id
	                                     , case when xpro.pro_adj_price is null
			                                    then (xpri.before_price + xpri.adjust_price) 
			                                    else (xpri.before_price + xpro.pro_adj_price) + xpri.adjust_price end current_price
                                    from (
                                        select a.doc_no, a.comp_code, a.brn_code, b.pd_id, b.before_price, b.adjust_price 
                                        from dbo.oil_standard_price_hd a, dbo.oil_standard_price_dt b
                                        where a.brn_code = '{ brnCode }'	
                                          and a.comp_code = '{ compCode }'		
                                          and convert(varchar(10),a.effective_date,105) = 
                                            (
                                            select convert(varchar(10),max(c.effective_date),105)
                                            from dbo.oil_standard_price_hd c
                                            where c.effective_date <= convert(datetime, '{ sysDateTime }', 103)	
                                              and c.approve_status = 'Y'
                                              and c.brn_code = a.brn_code
                                              and c.doc_type = a.doc_type
                                              and c.comp_code = a.comp_code
                                            )
                                          and a.doc_no = b.doc_no
                                          and a.brn_code = b.brn_code
                                          and a.comp_code = b.comp_code
                                     ) xpri left join
                                     (
                                        select a.comp_code, a.brn_code, b.pd_id, sum(b.adjust_price) as pro_adj_price
                                        from dbo.oil_promotion_price_hd a, dbo.oil_promotion_price_dt b
                                        where a.brn_code = '{ brnCode }'		-- parameter: branch
                                          and a.comp_code = '{ brnCode }'		-- parameter: branch
                                          and a.doc_no in
                                            (
                                            select c.doc_no
                                            from dbo.oil_promotion_price_hd c
                                            where ( c.start_date <= convert(datetime, '{ sysDateTime }', 103) 	
                                              and c.finish_date >= convert(datetime, '{ sysDateTime }', 103) )	
                                              and c.approve_status = 'Y'
                                              and c.brn_code = a.brn_code
                                              and c.comp_code = a.comp_code
                                            )
                                          and a.doc_no = b.doc_no
                                          and a.brn_code = b.brn_code
                                          and a.comp_code = b.comp_code
                                        group by a.comp_code, a.brn_code, b.pd_id
                                     ) xpro 
                                    on xpri.brn_code = xpro.brn_code 
                                    and xpri.pd_id = xpro.pd_id
                                    and xpri.comp_code = xpro.comp_code;";
            return result;
        }
        
        [HttpGet("GetMasBranchConfig/{pStrCompCode}/{pStrBrnCode}")]
        public async Task<IActionResult> GetMasBranchConfig(string pStrCompCode, string pStrBrnCode)
        {
            return await DefaultService.DoActionAsync(
                "GetMasBranchConfig", 
                async () => await _branchService.GetMasBranchConfig(pStrCompCode, pStrBrnCode), 
                log);
        }


        [HttpPost("UpdateTest")]
        public async Task<IActionResult> UpdateTest([FromBody] TestRequest request)
        {
            ResponseData<MasBranch> response = new ResponseData<MasBranch>();

            try
            {
                var branch = this.context.MasBranches.FirstOrDefault(x => x.BrnCode == request.BrnCode);
                if(branch != null)
                {
                    branch.UpdatedDate = DateTime.Now;
                    branch.UpdatedBy = request.DocDate.ToString("yyyyMMdd");
                    this.context.SaveChanges();

                }

                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }


    }
}
