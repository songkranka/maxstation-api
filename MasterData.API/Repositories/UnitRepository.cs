using log4net;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class UnitRepository : SqlDataAccessHelper, IUnitRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BranchRepository));

        public UnitRepository(PTMaxstationContext context) : base(context)
        {

        }

        public async Task<QueryResult<MasUnit>> ListAsync(UnitQuery pQuery)
        {
            var qryUnit = context.MasUnits
             .OrderBy(x => x.UnitId)
             .AsNoTracking();


            if (!string.IsNullOrEmpty(pQuery.Keyword))
            {
                qryUnit = qryUnit.Where(p => p.UnitId.Contains(pQuery.Keyword)
                || p.MapUnitId.Contains(pQuery.Keyword)
                || p.UnitName.Contains(pQuery.Keyword));
            }

            int totalItems = await qryUnit.CountAsync();
            var resp = await qryUnit.Skip((pQuery.Page - 1) * pQuery.ItemsPerPage)
                                           .Take(pQuery.ItemsPerPage)
                                           .ToListAsync();
            
            return new QueryResult<MasUnit>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = pQuery.ItemsPerPage
            };

        }

        public async Task<MasUnit> GetMasUnitAsync(string unitId)
        {
            return await context.MasUnits.FirstOrDefaultAsync(s => s.UnitId == unitId);
        }

        public async Task AddUnitAsync(MasUnit masUnit)
        {
            await context.MasUnits.AddAsync(masUnit);
        }

        public async Task UpdateUnitAsync(MasUnit masUnit)
        {
            var result = await context.MasUnits.SingleOrDefaultAsync(x => x.MapUnitId == masUnit.MapUnitId);

            if (result != null)
            {
                result.MapUnitId = masUnit.MapUnitId;
                result.UnitName = masUnit.UnitName;
                result.UpdatedDate = masUnit.UpdatedDate;
                result.UpdatedBy = masUnit.UpdatedBy;
                context.SaveChanges();
            }
        }

        public async Task UpdateStatusAsync(MasUnit masUnit)
        {
            var result = await context.MasUnits.SingleOrDefaultAsync(x => x.MapUnitId == masUnit.MapUnitId);

            if (result != null)
            {
                result.UnitStatus = masUnit.UnitStatus;
                result.UpdatedDate = masUnit.UpdatedDate;
                result.UpdatedBy = masUnit.UpdatedBy;
                context.SaveChanges();
            }
        }
    }
}
