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
    public class TransferOutRepository : SqlDataAccessHelper, ITransferOutRepository
    {
        public TransferOutRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<List<InvTranoutHd>> ListTransferOutAsync(TransferOutQuery query)
        {
            List<InvTranoutHd> heads = new List<InvTranoutHd>();
            heads = await this.context.InvTranoutHds.Where(x => x.CompCode == query.CompCode
                                                               && x.BrnCode == query.BrnCode
                                                               && x.DocDate == query.DocDate
                                                                ).ToListAsync();


            heads.ForEach(hd => hd.InvTranoutDt = this.context.InvTranoutDts.Where(x => x.CompCode == hd.CompCode
                                                                                 && x.BrnCode == hd.BrnCode
                                                                                 && x.LocCode == hd.LocCode
                                                                                 && x.DocNo == hd.DocNo).Select(x => new InvTranoutDt
                                                                                 {
                                                                                     CompCode = x.CompCode,
                                                                                     BrnCode = x.BrnCode,
                                                                                     LocCode = x.LocCode,
                                                                                     DocNo = x.DocNo,
                                                                                     ItemQty = x.ItemQty,
                                                                                     PdId = x.PdId,
                                                                                     PdName = x.PdName,
                                                                                     RefQty = x.RefQty,
                                                                                     SeqNo = x.SeqNo,
                                                                                     StockQty = x.StockQty,
                                                                                     StockRemain = x.StockRemain,
                                                                                     UnitBarcode = x.UnitBarcode,
                                                                                     UnitId = x.UnitId,
                                                                                     UnitName  = x.UnitName
                                                                                 }).ToList()
            );

            return heads;
        }
    }
}
