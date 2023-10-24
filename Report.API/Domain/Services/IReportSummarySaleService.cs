using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System.Collections.Generic;
using System.IO;


namespace Report.API.Domain.Services
{
    public interface IReportSummarySaleService
    {
        MemoryStream GetReportSummarySaleExcel(ReportSummarySaleRequest req);
        ResponseSummarySaleForPDF GetReportSummarySalePDF(ReportSummarySaleRequest req);
        List<GetPeriodResponse> GetPeriod(ReportSummarySaleRequest req);
    }    
}
