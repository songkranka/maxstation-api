using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Repositories
{
    public interface IReportAuditRepository 
    {
        ReportAuditDiffResponse GetAuditDiffPDF(string compCode, string brnCode, string docNo);
        ReportAuditDetailResponse GetAuditDetailPDF(string compCode, string brnCode, string docNo);
    }
}
