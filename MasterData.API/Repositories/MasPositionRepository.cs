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
    public class MasPositionRepository : SqlDataAccessHelper, IMasPositionRepository
    {
        public MasPositionRepository(PTMaxstationContext context) : base(context) { }

        public async Task<List<MasPosition>> GetAll()
        {
            var response = new List<MasPosition>();
            response = await context.MasPositions.AsNoTracking()
                .OrderBy(x => x.PositionCode)
                .ToListAsync();
            return response;
        }
    }
}
