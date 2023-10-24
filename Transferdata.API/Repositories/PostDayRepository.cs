using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Response;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Helpers;
using Transferdata.API.Resources.PostDay;

namespace Transferdata.API.Repositories
{
    public class PostDayRepository : SqlDataAccessHelper,IPostDayRepository
    {
        public PostDayRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<DepositResponse> GetDepositAmt(PostDayQueryResource query)
        {
            var response = new DepositResponse();

            var result = await(from dp in context.DopPostdayHds
                                   join mb in context.MasBranches on new { a = dp.CompCode, b = dp.BrnCode, c = dp.LocCode } equals new { a = mb.CompCode, b = mb.BrnCode, c = mb.LocCode }
                                   where mb.BrnCode == query.BrnCode && dp.DocDate == query.DocDate
                                   select new DepositResponse()
                                   {
                                       DepositAmt = dp.DepositAmt
                                   }).AsNoTracking().FirstOrDefaultAsync();

            response.DepositAmt = result.DepositAmt;            
            return response;
        }
    }
}
