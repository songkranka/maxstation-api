using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Services
{
    public interface IReportStockService
    {
        MemoryStream GetReportStockExcel(ReportStockRequest req);
        Task<ReportStockResponse> GetReportStockPDF(ReportStockRequest req);
        ReportStockResponse GetMonthlyPDF(ReportStockRequest req);
        MemoryStream GetMonthlyExcel(ReportStockRequest req);

        ReportStockResponse GetReportStockTest(ReportStockRequest req);
    }

}
