using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Services
{
    public class ReportStockService : IReportStockService
    {
        private readonly IReportStockRepository _reportStockRepository;
        public ReportStockService(
            IReportStockRepository reportStockRepository)
        {
            _reportStockRepository = reportStockRepository;
        }

        public MemoryStream GetMonthlyExcel(ReportStockRequest req)
        {
            return _reportStockRepository.GetMonthlyExcel(req);
        }

        public ReportStockResponse GetMonthlyPDF(ReportStockRequest req)
        {
            return _reportStockRepository.GetMonthlyPDF(req);
        }

        public MemoryStream GetReportStockExcel(ReportStockRequest req)
        {
            return _reportStockRepository.GetReportStockExcel(req);
        }

        public async Task<ReportStockResponse> GetReportStockPDF(ReportStockRequest req)
        {
            return await _reportStockRepository.GetReportStockPDF(req);
        }

        public ReportStockResponse GetReportStockTest(ReportStockRequest req)
        {
            return _reportStockRepository.GetReportStockTest(req);
        }
    }
}
