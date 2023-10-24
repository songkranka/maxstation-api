using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
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
    public class MasControlRepository : SqlDataAccessHelper, IMasControlRepository
    {
        public MasControlRepository(PTMaxstationContext context) : base(context)
        {

        }

        public MasControl GetMasControl(MasControlRequest req) 
        {
            MasControl resp = this.context.MasControls.FirstOrDefault(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.CtrlCode == req.CtrlCode);
            return resp;
        }

        public async Task<MasControl> UpdateCtrlValueAsync(UpdateCtrlValueDateRequest request)
        {
            var result = await context.MasControls.SingleOrDefaultAsync(x => x.CompCode == request.CompCode && x.BrnCode == request.BrnCode && x.CtrlCode == "WDATE");

            if (result != null)
            {
                result.CtrlValue = request.SystemDate.AddDays(1).ToString("dd'/'MM'/'yyyy");
                result.UpdatedBy = request.User;
                result.UpdatedDate = DateTime.Now;
                context.SaveChanges();
            }
            return result;
        }
    }
}
