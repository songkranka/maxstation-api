using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MaxStation.Entities.Models;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;

namespace Report.API.Domain.Repositories
{
    public interface IReportSummarySaleRepository
    {
        MemoryStream GetReportSummarySaleExcel(ReportSummarySaleRequest req);
        ResponseSummarySaleForPDF GetReportSummarySalePDF(ReportSummarySaleRequest req);
        List<GetPeriodResponse> GetPeriod(ReportSummarySaleRequest req);
    }
}
