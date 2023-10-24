using MaxStation.Entities.Models;
using Report.API.Domain.Models.Queries;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        public ReportService(
            IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<QueryResult<SysReportConfig>> FindReportConfigByGroup(SysReportConfigQuery query)
        {
            return await _reportRepository.FindReportConfigByGroup(query);
        }

        public async Task<SysReportConfig> FindReportConfig(SysReportConfigByGroupQuery query)
        {
            return await _reportRepository.FindReportConfig(query);
        }
    }
}
