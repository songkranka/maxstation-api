using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using Report.API.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Services
{

    public class ReportSummarySaleService : IReportSummarySaleService
    {
        private readonly IReportSummarySaleRepository _ReportSummarySaleRepository;

        public ReportSummarySaleService(
            IReportSummarySaleRepository ReportSummarySaleRepository)
        {
            _ReportSummarySaleRepository = ReportSummarySaleRepository;
        }

        public List<GetPeriodResponse> GetPeriod(ReportSummarySaleRequest req)
        {
            return _ReportSummarySaleRepository.GetPeriod(req);
        }

        public MemoryStream GetReportSummarySaleExcel(ReportSummarySaleRequest req)
        {
            return _ReportSummarySaleRepository.GetReportSummarySaleExcel(req);
        }

        public ResponseSummarySaleForPDF GetReportSummarySalePDF(ReportSummarySaleRequest req)
        {
            return _ReportSummarySaleRepository.GetReportSummarySalePDF(req);
        }
    }
}
