using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Services
{
    public interface IInventoryService
    {
        List<InvWithdrawResponse> GetWithdrawPDF(InventoryRequest req);        
        MemoryStream GetWithdrawExcel(InventoryRequest req);
        List<InvReceiveProdResponse> GetReceiveProdPDF(InventoryRequest req);
        MemoryStream GetReceiveProdExcel(InventoryRequest req);
        List<InvTransferOutResponse> GetTransferOutPDF(InventoryRequest req);
        MemoryStream GetTransferOutExcel(InventoryRequest req);
        List<InvTransferInResponse> GetTransferInPDF(InventoryRequest req);
        MemoryStream GetTransferInExcel(InventoryRequest req);
        List<InvTransferCompareResponse> GetTransferComparePDF(InventoryRequest req);
        MemoryStream GetTransferCompareExcel(InventoryRequest req);
        List<InvTransferNotInResponse> GetTransferNotInPDF(InventoryRequest req);
        MemoryStream GetTransferNotInExcel(InventoryRequest req);        
        List<InvReturnSupResponse> GetReturnSupPDF(InventoryRequest req);
        MemoryStream GetReturnSupExcel(InventoryRequest req);
        List<InvReturnOilResponse> GetReturnOilPDF(InventoryRequest req);
        MemoryStream GetReturnOilExcel(InventoryRequest req);
        List<InvAdjustResponse> GetAdjustPDF(InventoryRequest req);
        MemoryStream GetAdjustExcel(InventoryRequest req);

    }

}
