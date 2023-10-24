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
    public class MasMappingRepository : SqlDataAccessHelper, IMasMappingRepository
    {
        public MasMappingRepository(PTMaxstationContext context) : base(context) { }

        public async Task<List<MasMapping>> GetMasMapping(string mapValue)
        {
            var response = new List<MasMapping>();
            response = await context.MasMappings.AsNoTracking().Where(x => x.MapValue == mapValue && x.MapStatus == "Active")
                .OrderBy(x => x.MapId)
                .ToListAsync();
            return response;
        }
    }
}
