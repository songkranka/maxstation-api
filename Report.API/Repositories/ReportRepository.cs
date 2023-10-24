using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Report.API.Domain.Models.Queries;
using Report.API.Domain.Repositories;
using Report.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Repositories
{
    public class ReportRepository : SqlDataAccessHelper, IReportRepository
    {
        public ReportRepository(PTMaxstationContext context) : base(context)
        {

        }

        public async Task<QueryResult<SysReportConfig>> FindReportConfigByGroup(SysReportConfigQuery query)
        {
            var queryable = context.SysReportConfigs
                .Where(x => x.ReportGroup == query.Group && x.ReportStatus == "Active")
                .OrderBy(x => x.SeqNo)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.ReportName.Contains(query.Keyword));
            }

            int totalItems = await queryable.CountAsync();
            //var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
            //                               .Take(query.ItemsPerPage)
            //                               .ToListAsync();
            var resp = await queryable.ToListAsync();

            return new QueryResult<SysReportConfig>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }

        public async Task<SysReportConfig> FindReportConfig(SysReportConfigByGroupQuery query)
        {
            var reportConfig = await context.SysReportConfigs.FirstOrDefaultAsync(x => x.ReportGroup == query.Group);
            return reportConfig;
        }
    }
}
