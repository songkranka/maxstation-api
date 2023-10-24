using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaxStation.Utility.Helpers.CollectLogError;

namespace MasterData.API.Repositories
{
    public class CostCenterRepository : SqlDataAccessHelper, ICostCenterRepository
    {
        public CostCenterRepository(PTMaxstationContext context) : base(context)
        {
        }

        public List<MasCostCenter> GetCostCenterList(CostCenterRequest req)
        {
            var masCostCenters = context.MasCostCenters.Where(
                x => (x.CompCode == req.CompCode && req.BrnCode == req.BrnCode && x.BrnStatus == "Active")
            ).ToList();
            return masCostCenters;
        }

        public async Task<QueryResult<MasCostCenter>> ListAsync(CosCenterQuery query)
        {
            var queryable = context.MasCostCenters
                .Where(x => x.CompCode == query.CompCode)
                .OrderBy(x => x.BrnCode)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.BrnName.Contains(query.Keyword) || p.BrnCode.Contains(query.Keyword));
            }


            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                .Take(query.ItemsPerPage)
                .ToListAsync();

            return new QueryResult<MasCostCenter>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }

        public async Task<MasCostCenter> GetCostCenterByGuid(Guid guid)
        {
            var masCostCenter = await context.MasCostCenters.FirstOrDefaultAsync(x => x.Guid == guid);
            return masCostCenter;
        }

        public async Task AddCostCenterAsync(MasCostCenter costCenter)
        {
            await context.MasCostCenters.AddAsync(costCenter);
        }

        public async Task SaveLogError(LogError logError)
        {
            await context.LogErrors.AddAsync(logError);
        }

        public async Task UpdateCostCenterAsync(MasCostCenter costCenter)
        {
            var result = await context.MasCostCenters.SingleOrDefaultAsync(x => x.CompCode == costCenter.CompCode && x.BrnCode == costCenter.BrnCode);

            if (result != null)
            {
                result.MapBrnCode = costCenter.MapBrnCode;
                result.BrnName = costCenter.BrnName;
                result.UpdatedDate = costCenter.UpdatedDate;
                result.UpdatedBy = costCenter.UpdatedBy;
                context.SaveChanges();
            }
        }

        public async Task<MasCostCenter> CheckMasCostCenter(MasCostCenter request)
        {
            var masCostCenter = await context.MasCostCenters.FirstOrDefaultAsync(x => x.CompCode == request.CompCode && x.BrnCode == request.BrnCode && x.BrnName == request.BrnName && x.MapBrnCode == request.MapBrnCode);
            return masCostCenter;
        }

        public async Task<MasCostCenter> GetMasConstCenterFromBranCodeAndCompCode(string compCode, string brnCode)
        {
            return await context.MasCostCenters.FirstOrDefaultAsync(s => s.BrnCode == brnCode && s.CompCode == compCode);
        }
    }
}