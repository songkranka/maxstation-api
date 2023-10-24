using MaxStation.Entities.Models;
using Report.API.Domain.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Repositories
{
    public interface IReportRepository
    {
        Task<QueryResult<SysReportConfig>> FindReportConfigByGroup(SysReportConfigQuery query);
        Task<SysReportConfig> FindReportConfig(SysReportConfigByGroupQuery query);
    }
}
