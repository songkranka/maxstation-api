using MaxStation.Entities.Models;
using Report.API.Domain.Models.Requests;
using Report.API.Resources.ReportSummaryOilBalance;
using System.Collections.Generic;
using System.IO;


namespace Report.API.Domain.Services
{
    public interface IReportSummaryOilBalanceService
    {
        MemoryStream GetReportSummaryOilBalanceExcel(ReportSummaryOilBalanceRequest req);
        ResponseTankModelForPDF GetReportSummaryOilBalancePDF(ReportSummaryOilBalanceRequest req);
    }
}
