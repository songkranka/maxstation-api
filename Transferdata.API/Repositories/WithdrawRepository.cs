using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Helpers;


namespace Transferdata.API.Repositories
{
    public class WithdrawRepository : SqlDataAccessHelper, IWithdrawRepository
    {
        public WithdrawRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<List<InvWithdrawHd>> ListWithdrawAsync(WithdrawQuery query)
        {
            List<InvWithdrawHd> heads = new List<InvWithdrawHd>();
            heads = await this.context.InvWithdrawHds.Where(x => x.CompCode == query.CompCode
                                                               && x.BrnCode == query.BrnCode
                                                               && x.DocDate == query.DocDate
                                                                ).ToListAsync();


            heads.ForEach(hd => hd.InvWithdrawDt = this.context.InvWithdrawDts.Where(x => x.CompCode == hd.CompCode
                                                                                 && x.BrnCode == hd.BrnCode
                                                                                 && x.LocCode == hd.LocCode
                                                                                 && x.DocNo == hd.DocNo).Select(x => new InvWithdrawDt
                                                                                 {
                                                                                     CompCode = x.CompCode,
                                                                                     BrnCode = x.BrnCode,
                                                                                     LocCode = x.LocCode,
                                                                                     DocNo = x.DocNo,
                                                                                     ItemQty = x.ItemQty,
                                                                                     PdId = x.PdId,
                                                                                     PdName = x.PdName,                                                                                     
                                                                                     SeqNo = x.SeqNo,
                                                                                     StockQty = x.StockQty,                                                                                     
                                                                                     UnitBarcode = x.UnitBarcode,
                                                                                     UnitId = x.UnitId,
                                                                                     UnitName = x.UnitName                                                                                                                                                                         
                                                                                 }).ToList()
            );

            return heads;
        }
    }
}
