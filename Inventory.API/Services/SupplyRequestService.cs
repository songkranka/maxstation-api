using Inventory.API.Domain.Models;
using Inventory.API.Domain.Repositories;
using Inventory.API.Domain.Services;
using Inventory.API.Resources.SupplyRequest;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Services
{
    public class SupplyRequestService : ISupplyRequestService
    {
        private readonly ISupplyRequestRepository _supplyRequestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SupplyRequestService(
            ISupplyRequestRepository supplyRequestRepository,
            IUnitOfWork unitOfWork)
        {
            _supplyRequestRepository = supplyRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SupplyRequest>> GetRequestDataByRequest(SupplyRequestByRequestQuery request)
        {
            //return await _receiveOilRepository.ReceiveOilHdListAsync(query);

            List<SupplyRequest> output = new List<SupplyRequest>()
            {
                new SupplyRequest { CompCode  = "B", BrnCode = "822", BrnName = "", WhCode = "", WhName = "", SeqNo = 1, PdId = "300000150", PdName = "KolorKut ครีมวัดน้ำมัน(Gasoline Gauging)", RequestQty = 1, ItemQty = 1, UnitName = ""},
                new SupplyRequest { CompCode  = "B", BrnCode = "822", BrnName = "", WhCode = "", WhName = "", SeqNo = 2, PdId = "300000151", PdName = "Sargel ครีมวัดน้ำ (Water Finding Paste)", RequestQty = 2, ItemQty = 1, UnitName = ""},
                new SupplyRequest { CompCode  = "B", BrnCode = "822", BrnName = "", WhCode = "", WhName = "", SeqNo = 3, PdId = "300000152", PdName = "กระเป๋าคาดเอว พนักงานหน้าลาน", RequestQty = 3, ItemQty = 3, UnitName = ""}
            };
            return output;
        }

        //public async Task SaveExcelFile(List<SupplyRequest> supplyRequests, FileInfo file)
        //{
        //    DeleteIfExists(file);

        //    using var package = new ExcelPackage(file);
        //    var ws = package.Workbook.Worksheets.Add("Supply");
        //    var range = ws.Cells["A1"].LoadFromCollection(supplyRequests, true);
        //    range.AutoFitColumns();

        //    ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //    ws.Row(1).Style.Font.Name = "Tahoma";
        //    ws.Row(1).Style.Font.Size = 10;

        //    await package.SaveAsync();


        //}

        public List<SupplyRequest> LoadExcelFile(IFormFile file)
        {
            var stream = file.OpenReadStream();
            List<SupplyRequest> supplyRequests = new List<SupplyRequest>();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.First();
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var compCode = worksheet.Cells[row, 1].Value?.ToString();
                    var brnCode = worksheet.Cells[row, 2].Value?.ToString();
                    var brnName = worksheet.Cells[row, 3].Value?.ToString();
                    var whCode = worksheet.Cells[row, 4].Value?.ToString();
                    var whName = worksheet.Cells[row, 5].Value?.ToString();
                    var seqNo = worksheet.Cells[row, 6].Value?.ToString();
                    var pdId = worksheet.Cells[row, 7].Value?.ToString();
                    var pdName = worksheet.Cells[row, 8].Value?.ToString();
                    var requestQty = worksheet.Cells[row, 9].Value?.ToString();
                    var itemQty = worksheet.Cells[row, 10].Value?.ToString();
                    var unitName = worksheet.Cells[row, 11].Value?.ToString();

                    var supplyRequst = new SupplyRequest()
                    {
                        CompCode = compCode,
                        BrnCode = brnCode,
                        BrnName = brnName,
                        WhCode = whCode,
                        WhName = whName,
                        SeqNo = string.IsNullOrEmpty(seqNo) ? 0 : int.Parse(seqNo),
                        PdId = pdId,
                        PdName = pdName,
                        RequestQty = string.IsNullOrEmpty(requestQty) ? 0 : int.Parse(requestQty),
                        ItemQty = string.IsNullOrEmpty(itemQty) ? 0 : int.Parse(itemQty),
                        UnitName = unitName
                    };

                    supplyRequests.Add(supplyRequst);
                }
            }

            return supplyRequests;
        }
    }
}
