using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Response;
using Transferdata.API.Domain.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Helpers;
using Transferdata.API.Resources.Pos;

namespace Transferdata.API.Repositories
{
    public class PosRepository : SqlDataAccessHelper, IPosRepository
    {
        public PosRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<TranferPosResponse> GetDepositAmt(TranferPosQueryResource query)
        {
            var response = new TranferPosResponse();
            
            var depositAmt = await (from dp in context.DopPostdayHds
                                    join mb in context.MasBranches on new { a = dp.CompCode, b = dp.BrnCode, c = dp.LocCode } equals new { a = mb.CompCode, b = mb.BrnCode, c = mb.LocCode }
                                    where mb.BrnCode == query.BrnCode && dp.DocDate == query.DocDate
                                    select new TranferPosResponse()
                                    {
                                        DepositAmt = dp.DepositAmt
                                    }).AsNoTracking().FirstOrDefaultAsync();
            return response;
        }
    }
}
