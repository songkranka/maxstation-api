using log4net;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class MasBranchConfigRepository : SqlDataAccessHelper, IMasBranchConfigRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BranchRepository));

        public MasBranchConfigRepository(PTMaxstationContext context) : base(context) { }

        public async Task<MasBranchConfig> GetMasBranchConfig(string compCode, string brnCode)
        {
            return await context.MasBranchConfigs.FirstOrDefaultAsync(x => x.CompCode == compCode && x.BrnCode == brnCode);
        }

        public async Task AddMasBranchConfigAsync(MasBranchConfig masBranchConfig)
        {
            await context.MasBranchConfigs.AddAsync(masBranchConfig);
        }

        public async Task UpdateMasBranchConfigAsync(MasBranchConfig masBranchConfig)
        {
            var result = await context.MasBranchConfigs.SingleOrDefaultAsync(x => x.CompCode == masBranchConfig.CompCode && x.BrnCode == masBranchConfig.BrnCode);
            if (result != null)
            {
                result.IsLockMeter = masBranchConfig.IsLockMeter;
                result.IsLockSlip = masBranchConfig.IsLockSlip;
                result.UpdatedDate = masBranchConfig.UpdatedDate;
                result.UpdatedBy = masBranchConfig.UpdatedBy;
                context.SaveChanges();
            }
        }

        public async Task DeleteMasBranchConfigAsync(string compCode, string brnCode)
        {
            var result = await context.MasBranchConfigs.Where(x => x.CompCode == compCode && x.BrnCode == brnCode).ToListAsync();

            if (result != null)
            {
                context.MasBranchConfigs.RemoveRange(result);
                context.SaveChanges();
            }
        }
    }
}
