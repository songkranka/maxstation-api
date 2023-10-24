using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Services;
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
    public class DocumentTypeController : BaseController
    {
        private readonly IDocumentTypeService _documentTypeService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));

        public DocumentTypeController(
            IDocumentTypeService documentTypeService,
            PTMaxstationContext context) : base(context)
        {
            _documentTypeService = documentTypeService;
        }

        [HttpPost("GetDocumentType")]
        public IActionResult GetPattern([FromBody] DocumentTypeRequest req)
        {
            ResponseData<List<MasDocumentType>> response = new ResponseData<List<MasDocumentType>>();
            try
            {
                response.Data = _documentTypeService.GetDocumentTypeByDocType(req);
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
    }
}
