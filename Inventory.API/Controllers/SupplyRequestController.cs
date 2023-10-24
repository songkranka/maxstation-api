using AutoMapper;
using Inventory.API.Domain.Models;
using Inventory.API.Domain.Services;
using Inventory.API.Resources.SupplyRequest;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SupplyRequestController : BaseController
    {
        private readonly ISupplyRequestService _supplyRequestService;
        private readonly IMapper _mapper;
        private static readonly ILog _log = LogManager.GetLogger(typeof(ReceiveGasController));
        public SupplyRequestController(
            ISupplyRequestService supplyRequestService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _supplyRequestService = supplyRequestService;
            _mapper = mapper;

        }

        [HttpPost("WriteExcelByRequest")]
        public async Task<IActionResult> WriteExcelByRequest([FromBody] SupplyRequestByRequestQuery request)
        {
            try
            {
                var supplyRequests = await _supplyRequestService.GetRequestDataByRequest(request);
                var stream = new MemoryStream();

                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Request");
                    var customStyle = xlPackage.Workbook.Styles.CreateNamedStyle("CustomStyle");
                    customStyle.Style.Font.UnderLine = true;
                    customStyle.Style.Font.Color.SetColor(Color.Red);

                    var startRow = 1;
                    var row = startRow;

                    //worksheet.Cells["A1"].Value = "Supply Request";
                    //using (var r = worksheet.Cells["A1:C1"])
                    //{
                    //    r.Merge = true;
                    //    r.Style.Font.Color.SetColor(Color.Green);
                    //    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    //    r.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(23, 55, 93));
                    //}

                    worksheet.Cells["A1"].Value = "COMP_CODE";
                    worksheet.Cells["B1"].Value = "BRN_CODE";
                    worksheet.Cells["C1"].Value = "BRN_NAME";
                    worksheet.Cells["D1"].Value = "WH_CODE";
                    worksheet.Cells["E1"].Value = "WH_NAME";
                    worksheet.Cells["F1"].Value = "SEQ_NO";
                    worksheet.Cells["G1"].Value = "PD_ID";
                    worksheet.Cells["H1"].Value = "PD_NAME";
                    worksheet.Cells["I1"].Value = "REQUEST_QTY";
                    worksheet.Cells["J1"].Value = "ITEM_QTY";
                    worksheet.Cells["K1"].Value = "UNIT_NAME";

                    worksheet.Cells["A1:K1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells["A1:K1"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                    row = 2;

                    foreach (var supplyRequest in supplyRequests)
                    {
                        worksheet.Cells[row, 1].Value = supplyRequest.CompCode;
                        worksheet.Cells[row, 2].Value = supplyRequest.BrnCode;
                        worksheet.Cells[row, 3].Value = supplyRequest.BrnName;
                        worksheet.Cells[row, 4].Value = supplyRequest.WhCode;
                        worksheet.Cells[row, 5].Value = supplyRequest.WhName;
                        worksheet.Cells[row, 6].Value = supplyRequest.SeqNo;
                        worksheet.Cells[row, 7].Value = supplyRequest.PdId;
                        worksheet.Cells[row, 8].Value = supplyRequest.PdName;
                        worksheet.Cells[row, 8].Value = supplyRequest.RequestQty;
                        worksheet.Cells[row, 8].Value = supplyRequest.ItemQty;
                        worksheet.Cells[row, 8].Value = supplyRequest.UnitName;

                        row += 1;
                    }

                    xlPackage.Workbook.Properties.Title = "Supply request list";
                    xlPackage.Workbook.Properties.Author = "Max station";

                    xlPackage.Save();
                }

                stream.Position = 0;
                string excelName = $"Supply_Request-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to download excel", ex);
                return Ok();
            }
        }

        [HttpPost("LoadExcelFile")]
        public IActionResult LoadExcelFile(IFormFile fileRequest)
        {
            ResponseData<List<SupplyRequest>> response = new ResponseData<List<SupplyRequest>>();

            try
            {
                response.Data = _supplyRequestService.LoadExcelFile(fileRequest);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                _log.Error("Failed to upload excel", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }
    }
}
