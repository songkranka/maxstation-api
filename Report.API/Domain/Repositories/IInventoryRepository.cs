using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Repositories
{
    public interface IInventoryRepository
    {
        List<InvTransferInResponse> GetTransferInHdPDF(InventoryRequest req);
        List<InvTransferInResponse> GetTransferInDtPDF(InventoryRequest req);
        MemoryStream GetTransferInHdExcel(InventoryRequest req);
        MemoryStream GetTransferInDtExcel(InventoryRequest req);
        List<InvTransferCompareResponse> GetTransferComparePDF(InventoryRequest req);
        MemoryStream GetTransferCompareExcel(InventoryRequest req);
        List<InvTransferOutResponse> GetTransferOutHdPDF(InventoryRequest req);
        List<InvTransferOutResponse> GetTransferOutDtPDF(InventoryRequest req);
        MemoryStream GetTransferOutHdExcel(InventoryRequest req);
        MemoryStream GetTransferOutDtExcel(InventoryRequest req);
        List<InvTransferNotInResponse> GetTransferNotInPDF(InventoryRequest req);
        MemoryStream GetTransferNotInExcel(InventoryRequest req);
        List<InvWithdrawResponse> GetWithdrawHdPDF(InventoryRequest req);
        List<InvWithdrawResponse> GetWithdrawDtPDF(InventoryRequest req);
        MemoryStream GetWithdrawHdExcel(InventoryRequest req);
        MemoryStream GetWithdrawDtExcel(InventoryRequest req);

        List<InvReceiveProdResponse> GetReceiveProdHdPDF(InventoryRequest req);
        List<InvReceiveProdResponse> GetReceiveProdDtPDF(InventoryRequest req);
        MemoryStream GetReceiveProdHdExcel(InventoryRequest req);
        MemoryStream GetReceiveProdDtExcel(InventoryRequest req);

        List<InvReturnSupResponse> GetReturnSupHdPDF(InventoryRequest req);
        List<InvReturnSupResponse> GetReturnSupDtPDF(InventoryRequest req);
        MemoryStream GetReturnSupHdExcel(InventoryRequest req);
        MemoryStream GetReturnSupDtExcel(InventoryRequest req);

        List<InvReturnOilResponse> GetReturnOilHdPDF(InventoryRequest req);
        List<InvReturnOilResponse> GetReturnOilDtPDF(InventoryRequest req);
        MemoryStream GetReturnOilHdExcel(InventoryRequest req);
        MemoryStream GetReturnOilDtExcel(InventoryRequest req);

        List<InvAdjustResponse> GetAdjustHdPDF(InventoryRequest req);
        List<InvAdjustResponse> GetAdjustDtPDF(InventoryRequest req);
        MemoryStream GetAdjustHdExcel(InventoryRequest req);        
        MemoryStream GetAdjustDtExcel(InventoryRequest req);
    }
}
