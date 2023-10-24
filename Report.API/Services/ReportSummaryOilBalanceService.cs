using MaxStation.Entities.Models;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using Report.API.Domain.Services.Communication;
using Report.API.Resources.ReportSummaryOilBalance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Services
{

    public class ReportSummaryOilBalanceService : IReportSummaryOilBalanceService
    {
        private readonly IReportSummaryOilBalanceRepository _ReportSummaryOilBalanceRepository;

        public ReportSummaryOilBalanceService(
            IReportSummaryOilBalanceRepository ReportSummaryOilBalanceRepository)
        {
            _ReportSummaryOilBalanceRepository = ReportSummaryOilBalanceRepository;
        }

        public MemoryStream GetReportSummaryOilBalanceExcel(ReportSummaryOilBalanceRequest req)
        {
            return _ReportSummaryOilBalanceRepository.GetReportSummaryOilBalanceExcel(req);
        }

        public ResponseTankModelForPDF GetReportSummaryOilBalancePDF(ReportSummaryOilBalanceRequest req)
        {
            return _ReportSummaryOilBalanceRepository.GetReportSummaryOilBalancePDF(req);
        }
    }
}
