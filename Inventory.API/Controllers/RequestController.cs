using Inventory.API.Domain.Models;
using Inventory.API.Domain.Services;
using Inventory.API.Resources.Request;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : BaseController
    {
        private readonly IRequestService _requestService;
        private static readonly ILog log = LogManager.GetLogger(typeof(RequestController));
        public RequestController(
            IRequestService requestService,
            PTMaxstationContext context) : base(context)

        {
            _requestService = requestService;
        }

        [HttpPost("CreateRequest")]
        public async Task<IActionResult> CreateRequest([FromBody] InvRequest obj)
        {
            //Convert Date to TimeZone +7 Fixed ไว้ก่อน
            obj.DocDate = ((DateTime)obj.DocDate).AddHours(7);
            obj.CreatedDate = ((DateTime)obj.CreatedDate).AddHours(7);
            obj.UpdatedDate = ((DateTime)obj.UpdatedDate).AddHours(7);

            ResponseData<InvRequest> response = new ResponseData<InvRequest>();
            try
            {
                response.Data = await _requestService.CreateRequest(obj);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เพิ่มข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("GetRequestList")]
        public IActionResult GetRequestList([FromBody] RequestQueryResource req)
        {
            ResponseData<List<InvRequestHds>> response = new ResponseData<List<InvRequestHds>>();

            try
            {
                response.Data = _requestService.GetRequestHDList(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("GetRequestHdList")]
        public async Task<IActionResult> GetRequestHdList([FromBody] RequestData req)
        {
            //Convert Date to TimeZone +7 Fixed ไว้ก่อน
            req.DocDate = ((DateTime)req.DocDate).AddHours(7);
            if (req.FromDate != null)
            {
                req.FromDate = ((DateTime)req.FromDate).AddHours(7);
            }
            if (req.ToDate != null)
            {
                req.ToDate = ((DateTime)req.ToDate).AddHours(7);
            }

            ResponseData<List<InvRequestHds>> response = new ResponseData<List<InvRequestHds>>();
            try
            {
                response.Data = await _requestService.GetRequestHDListNew(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                response.totalItems = await _requestService.GetRequestHDCount(req); ;
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                response.totalItems = 0;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }

        [HttpPost("GetRequest")]
        public IActionResult GetRequest([FromBody] RequestQueryResource req)
        {
            ResponseData<InvRequest> response = new ResponseData<InvRequest>();
            try
            {
                response.Data = _requestService.GetRequest(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPut("UpdateRequest")]
        public async Task<IActionResult> UpdateRequest([FromBody] InvRequest obj)
        {
            ResponseData<InvRequest> response = new ResponseData<InvRequest>();
            try
            {
                response.Data = await _requestService.UpdateRequest(obj);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เพิ่มข้อมูลสำเร็จ";
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
