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

    public class ReceiveProdRepository : SqlDataAccessHelper, IReceiveProdRepository
    {
        public ReceiveProdRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<List<InvReceiveProdHd>> ListReceiveGasAsync(ReceiveProdQuery query)
        {
            List<InvReceiveProdHd> heads = new List<InvReceiveProdHd>();
            heads = await this.context.InvReceiveProdHds.Where(x => x.CompCode == query.CompCode
                                                               && x.BrnCode == query.BrnCode
                                                               && x.DocDate == query.DocDate
                                                               && x.DocType == "Gas"
                                                                ).ToListAsync();


            heads.ForEach(hd => hd.InvReceiveProdDt = this.context.InvReceiveProdDts.Where(x => x.CompCode == hd.CompCode
                                                                                 && x.BrnCode == hd.BrnCode
                                                                                 && x.LocCode == hd.LocCode
                                                                                 && x.DocNo == hd.DocNo).Select(x => new InvReceiveProdDt
                                                                                 {
                                                                                     CompCode = x.CompCode,
                                                                                     BrnCode = x.BrnCode,
                                                                                     LocCode = x.LocCode,
                                                                                     DocType = x.DocType,
                                                                                     Density = x.Density,
                                                                                     DensityBase = x.DensityBase,
                                                                                     DiscAmt = x.DiscAmt,
                                                                                     DiscAmtCur = x.DiscAmtCur,
                                                                                     DiscHdAmt = x.DiscHdAmt,
                                                                                     DiscHdAmtCur = x.DiscHdAmtCur,
                                                                                     DocNo = x.DocNo,
                                                                                     IsFree = x.IsFree,
                                                                                     ItemQty = x.ItemQty,
                                                                                     ItemRemain = x.ItemRemain,
                                                                                     PdId = x.PdId,
                                                                                     PdName = x.PdName,
                                                                                     PoQty = x.PoQty,
                                                                                     ReturnQty = x.ReturnQty,
                                                                                     ReturnStock = x.ReturnStock,
                                                                                     SeqNo = x.SeqNo,
                                                                                     StockQty = x.StockQty,
                                                                                     StockRemain = x.StockRemain,
                                                                                     SubAmt = x.SubAmt,
                                                                                     SubAmtCur = x.SubAmtCur,
                                                                                     SumItemAmt = x.SumItemAmt,
                                                                                     SumItemAmtCur = x.SumItemAmtCur,
                                                                                     TaxBaseAmt = x.TaxBaseAmt,
                                                                                     TaxBaseAmtCur = x.TaxBaseAmtCur,
                                                                                     Temperature = x.Temperature,
                                                                                     TotalAmt = x.TotalAmt,
                                                                                     TotalAmtCur = x.TotalAmtCur,
                                                                                     UnitBarcode = x.UnitBarcode,
                                                                                     UnitId = x.UnitId,
                                                                                     UnitName = x.UnitName,
                                                                                     UnitPrice = x.UnitPrice,
                                                                                     UnitPriceCur = x.UnitPriceCur,
                                                                                     VatAmt = x.VatAmt,
                                                                                     VatAmtCur = x.VatAmtCur,
                                                                                     VatRate = x.VatRate,
                                                                                     VatType = x.VatType,
                                                                                     WeightPrice = x.WeightPrice,
                                                                                     WeightQty = x.WeightQty
                                                                                 }).ToList()
            );

            return heads;
        }

        public async Task<List<InvReceiveProdHd>> ListReceiveOilAsync(ReceiveProdQuery query)
        {
            List<InvReceiveProdHd> heads = new List<InvReceiveProdHd>();
            heads = await this.context.InvReceiveProdHds.Where(x => x.CompCode == query.CompCode
                                                               && x.BrnCode == query.BrnCode
                                                               && x.DocDate == query.DocDate
                                                               && x.DocType == "Oil"
                                                                ).ToListAsync();


            heads.ForEach(hd => hd.InvReceiveProdDt = this.context.InvReceiveProdDts.Where(x => x.CompCode == hd.CompCode
                                                                                 && x.BrnCode == hd.BrnCode
                                                                                 && x.LocCode == hd.LocCode
                                                                                 && x.DocNo == hd.DocNo).Select(x => new InvReceiveProdDt
                                                                                 {
                                                                                     CompCode = x.CompCode,
                                                                                     BrnCode = x.BrnCode,
                                                                                     LocCode = x.LocCode,
                                                                                     DocType = x.DocType,
                                                                                     Density = x.Density,
                                                                                     DensityBase = x.DensityBase,
                                                                                     DiscAmt = x.DiscAmt,
                                                                                     DiscAmtCur = x.DiscAmtCur,
                                                                                     DiscHdAmt = x.DiscHdAmt,
                                                                                     DiscHdAmtCur = x.DiscHdAmtCur,
                                                                                     DocNo = x.DocNo,
                                                                                     IsFree = x.IsFree,
                                                                                     ItemQty = x.ItemQty,
                                                                                     ItemRemain = x.ItemRemain,
                                                                                     PdId = x.PdId,
                                                                                     PdName = x.PdName,
                                                                                     PoQty = x.PoQty,
                                                                                     ReturnQty = x.ReturnQty,
                                                                                     ReturnStock = x.ReturnStock,
                                                                                     SeqNo = x.SeqNo,
                                                                                     StockQty = x.StockQty,
                                                                                     StockRemain = x.StockRemain,
                                                                                     SubAmt = x.SubAmt,
                                                                                     SubAmtCur = x.SubAmtCur,
                                                                                     SumItemAmt = x.SumItemAmt,
                                                                                     SumItemAmtCur = x.SumItemAmtCur,
                                                                                     TaxBaseAmt = x.TaxBaseAmt,
                                                                                     TaxBaseAmtCur = x.TaxBaseAmtCur,
                                                                                     Temperature = x.Temperature,
                                                                                     TotalAmt = x.TotalAmt,
                                                                                     TotalAmtCur = x.TotalAmtCur,
                                                                                     UnitBarcode = x.UnitBarcode,
                                                                                     UnitId = x.UnitId,
                                                                                     UnitName = x.UnitName,
                                                                                     UnitPrice = x.UnitPrice,
                                                                                     UnitPriceCur = x.UnitPriceCur,
                                                                                     VatAmt = x.VatAmt,
                                                                                     VatAmtCur = x.VatAmtCur,
                                                                                     VatRate = x.VatRate,
                                                                                     VatType = x.VatType,
                                                                                     WeightPrice = x.WeightPrice,
                                                                                     WeightQty = x.WeightQty
                                                                                 }).ToList()
            );

            return heads;
        }

        public async Task<List<InvReceiveProdHd>> ListReceiveProdAsync(ReceiveProdQuery query)
        {
            List<InvReceiveProdHd> heads = new List<InvReceiveProdHd>();
            heads = await this.context.InvReceiveProdHds.Where(x => x.CompCode == query.CompCode
                                                               && x.BrnCode == query.BrnCode
                                                               && x.DocDate == query.DocDate
                                                                ).ToListAsync();


            heads.ForEach(hd => hd.InvReceiveProdDt = this.context.InvReceiveProdDts.Where(x => x.CompCode == hd.CompCode
                                                                                 && x.BrnCode == hd.BrnCode
                                                                                 && x.LocCode == hd.LocCode
                                                                                 && x.DocNo == hd.DocNo).Select(x => new InvReceiveProdDt
                                                                                 {
                                                                                     CompCode = x.CompCode,
                                                                                     BrnCode = x.BrnCode,
                                                                                     LocCode = x.LocCode,
                                                                                     DocType = x.DocType,
                                                                                     Density = x.Density,
                                                                                     DensityBase = x.DensityBase,
                                                                                     DiscAmt = x.DiscAmt,
                                                                                     DiscAmtCur = x.DiscAmtCur,
                                                                                     DiscHdAmt = x.DiscHdAmt,
                                                                                     DiscHdAmtCur = x.DiscHdAmtCur,
                                                                                     DocNo = x.DocNo,
                                                                                     IsFree = x.IsFree,
                                                                                     ItemQty = x.ItemQty,
                                                                                     ItemRemain = x.ItemRemain,
                                                                                     PdId = x.PdId,
                                                                                     PdName = x.PdName,
                                                                                     PoQty = x.PoQty,
                                                                                     ReturnQty = x.ReturnQty,
                                                                                     ReturnStock = x.ReturnStock,
                                                                                     SeqNo = x.SeqNo,
                                                                                     StockQty = x.StockQty,
                                                                                     StockRemain = x.StockRemain,
                                                                                     SubAmt = x.SubAmt,
                                                                                     SubAmtCur = x.SubAmtCur,
                                                                                     SumItemAmt = x.SumItemAmt,
                                                                                     SumItemAmtCur = x.SumItemAmtCur,
                                                                                     TaxBaseAmt = x.TaxBaseAmt,
                                                                                     TaxBaseAmtCur = x.TaxBaseAmtCur,
                                                                                     Temperature = x.Temperature,
                                                                                     TotalAmt = x.TotalAmt,
                                                                                     TotalAmtCur = x.TotalAmtCur,
                                                                                     UnitBarcode = x.UnitBarcode,
                                                                                     UnitId = x.UnitId,
                                                                                     UnitName = x.UnitName,
                                                                                     UnitPrice = x.UnitPrice,
                                                                                     UnitPriceCur = x.UnitPriceCur,
                                                                                     VatAmt = x.VatAmt,
                                                                                     VatAmtCur = x.VatAmtCur,
                                                                                     VatRate = x.VatRate,
                                                                                     VatType = x.VatType,
                                                                                     WeightPrice = x.WeightPrice,
                                                                                     WeightQty = x.WeightQty
                                                                                 }).ToList()
            );

            return heads;
        }

    }
}
