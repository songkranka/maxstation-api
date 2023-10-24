using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MaxStation.Entities.Models;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Resources.ReportSummaryOilBalance;

namespace Report.API.Domain.Repositories
{
    public interface IReportSummaryOilBalanceRepository
    {
        MemoryStream GetReportSummaryOilBalanceExcel(ReportSummaryOilBalanceRequest req);
        ResponseTankModelForPDF GetReportSummaryOilBalancePDF(ReportSummaryOilBalanceRequest req);
    }
}
