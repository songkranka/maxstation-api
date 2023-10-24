using Report.API.Domain.Enums;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System.Collections.Generic;
using System.IO;

namespace Report.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository repo;
        public InventoryService(IInventoryRepository repository)
        {
            repo = repository;
        }

        public List<InvReceiveProdResponse> GetReceiveProdPDF(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetReceiveProdHdPDF(req) : repo.GetReceiveProdDtPDF(req);
        }

        public MemoryStream GetReceiveProdExcel(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetReceiveProdHdExcel(req) : repo.GetReceiveProdDtExcel(req);
        }

        public List<InvTransferInResponse> GetTransferInPDF(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetTransferInHdPDF(req) : repo.GetTransferInDtPDF(req);
        }

        public MemoryStream GetTransferInExcel(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetTransferInHdExcel(req) : repo.GetTransferInDtExcel(req);
        }

        public List<InvTransferOutResponse> GetTransferOutPDF(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetTransferOutHdPDF(req) : repo.GetTransferOutDtPDF(req);
        }
        public MemoryStream GetTransferOutExcel(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetTransferOutHdExcel(req) : repo.GetTransferOutDtExcel(req);
        }

        public List<InvWithdrawResponse> GetWithdrawPDF(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetWithdrawHdPDF(req) : repo.GetWithdrawDtPDF(req);
        }

        public List<InvReturnSupResponse> GetReturnSupPDF(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetReturnSupHdPDF(req) : repo.GetReturnSupDtPDF(req);
        }

        public MemoryStream GetReturnSupExcel(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetReturnSupHdExcel(req) : repo.GetReturnSupDtExcel(req);
        }

        public List<InvReturnOilResponse> GetReturnOilPDF(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetReturnOilHdPDF(req) : repo.GetReturnOilDtPDF(req);
        }

        public MemoryStream GetReturnOilExcel(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetReturnOilHdExcel(req) : repo.GetReturnOilDtExcel(req);
        }

        public MemoryStream GetWithdrawExcel(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetWithdrawHdExcel(req) : repo.GetWithdrawDtExcel(req);
        }

        public List<InvAdjustResponse> GetAdjustPDF(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetAdjustHdPDF(req) : repo.GetAdjustDtPDF(req);
        }

        public MemoryStream GetAdjustExcel(InventoryRequest req)
        {
            return (req.ReportType == ReportType.Summary) ? repo.GetAdjustHdExcel(req) : repo.GetAdjustDtExcel(req);
        }

        public List<InvTransferCompareResponse> GetTransferComparePDF(InventoryRequest req)
        {
            return  repo.GetTransferComparePDF(req);
        }

        public MemoryStream GetTransferCompareExcel(InventoryRequest req)
        {
            return repo.GetTransferCompareExcel(req);
        }

        public List<InvTransferNotInResponse> GetTransferNotInPDF(InventoryRequest req)
        {
            return repo.GetTransferNotInPDF(req);
        }

        public MemoryStream GetTransferNotInExcel(InventoryRequest req)
        {
            return repo.GetTransferNotInExcel(req);
        }
    }
}
