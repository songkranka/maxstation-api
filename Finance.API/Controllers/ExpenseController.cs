using AutoMapper;
using Finance.API.Domain.Models;
using Finance.API.Domain.Models.Queries;
using Finance.API.Domain.Models.Request;
using Finance.API.Domain.Models.Response;
using Finance.API.Domain.Services;
using Finance.API.Models;
using Finance.API.Resources;
using Finance.API.Resources.Expense;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Finance.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReceiveController));

        public ExpenseController(
            IExpenseService expenseService,
            IMapper mapper)

        {
            _expenseService = expenseService;
            _mapper = mapper;
        }

        [HttpPost("List")]
        public async Task<QueryResult<FinExpenseHd>> GetExpenseListAsync([FromBody] ExpenseQueryResource query)
        {
            var expenseQuery = _mapper.Map<ExpenseQueryResource, ExpenseQuery>(query);
            var resource = await _expenseService.ListAsync(expenseQuery);
            return resource;

        }

        [HttpGet("GetMasDocPattern/{compCode}/{brnCode}/{docType}/{docDate}")]
        public async Task<IActionResult> GetMasDocPattern(string compCode, string brnCode, string docType, string docDate)
        {
            ResponseData<string> response = new ResponseData<string>();

            try
            {
                response.Data = await _expenseService.GetMasDocPattern(compCode, brnCode, docType, docDate);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("GetMasExpenseTable/{status}/{compCode}/{brnCode}/{locCode}/{docNo}")]
        public async Task<IActionResult> GetMasExpenseTableAsync(string status, string compCode, string brnCode, string locCode, string docNo)
        {
            ResponseData<List<ExpenseTable>> response = new ResponseData<List<ExpenseTable>>();
            
            try
            {
                response.Data = await _expenseService.GetMasExpenseTableAsync(status, compCode, brnCode, locCode, docNo);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("GetExpenseHd/{compCode}/{brnCode}/{locCode}/{guId}")]
        public async Task<IActionResult> GetExpenseHd(string compCode, string brnCode, string locCode, Guid guId)
        {
            ResponseData<FinExpenseHd> response = new ResponseData<FinExpenseHd>();

            try
            {
                response.Data = await _expenseService.GetExpenseHdAsync(compCode, brnCode, locCode, guId);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("GetExpenseEssTable/{compCode}/{brnCode}/{locCode}/{docNo}")]
        public async Task<IActionResult> GetExpenseEssTable(string compCode, string brnCode, string locCode, string docNo)
        {
            ResponseData<List<ExpenseEssTable>> response = new ResponseData<List<ExpenseEssTable>>();

            try
            {
                response.Data = await _expenseService.GetExpenseEssTableAsync(compCode, brnCode, locCode, docNo);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("SaveExpense")]
        public async Task<IActionResult> SaveExpense([FromBody] SaveExpenseRequest request)
        {
            ResponseData<SaveExpenseRequest> response = new ResponseData<SaveExpenseRequest>();

            try
            {
                var result = await _expenseService.SaveExpenseHdAsync(request);

                if (!result.Success)
                {
                    //return BadRequest(new ErrorResource(result.Message));
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = result.Message;
                    return Ok(JsonConvert.SerializeObject(response));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] FinExpenseHd request)
        {
            ResponseData<FinExpenseHd> response = new ResponseData<FinExpenseHd>();

            try
            {
                var result = await _expenseService.UpdateExpenseHdStatusAsync(request);

                if (!result.Success)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = result.Message;
                    return Ok(JsonConvert.SerializeObject(response));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }
    }
}
