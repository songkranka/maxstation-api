using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Services
{
    public class ReportAuditService : IReportAuditService
    {
        private readonly IReportAuditRepository _reportAuditRepository;
        public ReportAuditService(
            IReportAuditRepository reportAuditRepository)
        {
            _reportAuditRepository = reportAuditRepository;
        }

        public ReportAuditDiffResponse GetAuditDiffPDF(string compCode, string brnCode, string docNo)
        {
            return _reportAuditRepository.GetAuditDiffPDF(compCode, brnCode, docNo);
        }

        public ReportAuditDetailResponse GetAuditDetailPDF(string compCode, string brnCode, string docNo)
        {
            return _reportAuditRepository.GetAuditDetailPDF(compCode, brnCode, docNo);
        }
    }
}
