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
    public class TransferInRepository : SqlDataAccessHelper, ITransferInRepository
    {
        public TransferInRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<List<InvTraninHd>> ListTransferInAsync(TransferInQuery query)
        {
            List<InvTraninHd> heads = new List<InvTraninHd>();
            heads = await this.context.InvTraninHds.Where(x => x.CompCode == query.CompCode
                                                               && x.BrnCode == query.BrnCode
                                                               && x.DocDate == query.DocDate
                                                                ).ToListAsync();


            heads.ForEach(hd => hd.InvTraninDt = this.context.InvTraninDts.Where(x => x.CompCode == hd.CompCode
                                                                                 && x.BrnCode == hd.BrnCode
                                                                                 && x.LocCode == hd.LocCode
                                                                                 && x.DocNo == hd.DocNo).Select(x => new InvTraninDt
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
                                                                                     UnitName = x.UnitName,
                                                                                     
                                                                                 }).ToList()
            );

            return heads;
        }
    }
}
