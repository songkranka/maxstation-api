using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Helpers;

namespace Transferdata.API.Repositories
{

    public class CreditsaleRepository : SqlDataAccessHelper, ICreditsaleRepository 
    {
        public CreditsaleRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<List<CreditsaleAmount>> ListOilAmountAsync(SummaryQuery query)
        {
            List<CreditsaleAmount> result = new List<CreditsaleAmount>();

            var resp = (from hd in this.context.SalCreditsaleHds
                        join dt in this.context.SalCreditsaleDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                        join prod in this.context.MasProducts on dt.PdId equals prod.PdId
                        where hd.DocStatus != "Cancel"
                        && hd.CreatedBy != "dummy"
                        && hd.DocDate == query.DocDate
                        && prod.GroupId == "0000" // oil
                        select new { hd, dt }
                         ).AsQueryable();

            result = await resp.GroupBy(x => new { x.hd.CompCode, x.hd.BrnCode, x.hd.DocDate})
                    .Select(x => new CreditsaleAmount
                    {
                        CompCode = x.Key.CompCode,
                        BrnCode = x.Key.BrnCode,
                        DocDate = x.Key.DocDate,
                        NetAmt = x.Sum(s => s.dt.SumItemAmt)//ยังไม่หักส่วนลด
                    }).ToListAsync();

            return result;
        }

        public async Task<List<SalCreditsaleHd>> ListCreditsaleAsync(CreditsaleQuery query)
        {
            List<SalCreditsaleHd> heads = new List<SalCreditsaleHd>();

            heads = await this.context.SalCreditsaleHds.Where(x =>  x.CreatedBy != "dummy"
                                           //&& x.Post == "P"
                                           // &&  x.DocStatus != "Cancel"                                            
                                           && x.CompCode == query.CompCode
                                           && x.BrnCode == query.BrnCode
                                           && x.DocDate == query.DocDate
                                            ).ToListAsync();

            heads.ForEach(hd => hd.SalCreditsaleDt = this.context.SalCreditsaleDts.Where(x => x.CompCode == hd.CompCode
                                                                                 && x.BrnCode == hd.BrnCode
                                                                                 && x.LocCode == hd.LocCode
                                                                                 && x.DocNo == hd.DocNo).Select(x => new SalCreditsaleDt
                                                                                 {
                                                                                     CompCode = x.CompCode,
                                                                                     BrnCode = x.BrnCode,
                                                                                     LocCode = x.LocCode,
                                                                                     DocType = x.DocType,
                                                                                     DocNo = x.DocNo,
                                                                                     SeqNo = x.SeqNo,
                                                                                     PoNo = x.PoNo??"",
                                                                                     LicensePlate = x.LicensePlate??"",
                                                                                     Mile = x.Mile??0,
                                                                                     PdId = x.PdId,
                                                                                     PdName = x.PdName,
                                                                                     IsFree = x.IsFree??false,
                                                                                     UnitId = x.UnitId,
                                                                                     UnitBarcode = x.UnitBarcode,
                                                                                     UnitName = x.UnitName,
                                                                                     MeterStart = x.MeterStart??0,
                                                                                     MeterFinish = x.MeterFinish??0,
                                                                                     ItemQty = x.ItemQty??decimal.Zero,
                                                                                     StockQty = x.StockQty??0,
                                                                                     UnitPrice = x.UnitPrice,
                                                                                     UnitPriceCur = x.UnitPriceCur,
                                                                                     RefPrice = x.RefPrice??0,
                                                                                     RefPriceCur = x.RefPriceCur??0,
                                                                                     SumItemAmt = x.SumItemAmt,
                                                                                     SumItemAmtCur = x.SumItemAmtCur,
                                                                                     DiscAmt = x.DiscAmt,
                                                                                     DiscAmtCur = x.DiscAmtCur,
                                                                                     DiscHdAmt = x.DiscHdAmt,
                                                                                     DiscHdAmtCur = x.DiscHdAmtCur,
                                                                                     SubAmt = x.SubAmt,
                                                                                     SubAmtCur = x.SubAmtCur,
                                                                                     VatType = x.VatType,
                                                                                     VatRate = x.VatRate,
                                                                                     VatAmt = x.VatAmt,
                                                                                     VatAmtCur = x.VatAmtCur,
                                                                                     TaxBaseAmt = x.TaxBaseAmt,
                                                                                     TaxBaseAmtCur = x.TaxBaseAmtCur,
                                                                                     TotalAmt = x.TotalAmt,
                                                                                     TotalAmtCur = x.TotalAmtCur                                                                                    
                                                                                 }).ToList()
            );
            

            return heads;
        }

        public async Task<List<CreditsaleAmount>> ListCreditsaleAmountAsync(SummaryQuery query)
        {
            List<CreditsaleAmount> result = new List<CreditsaleAmount>();

            var resp = (from hd in this.context.SalCreditsaleHds
                        join dt in this.context.SalCreditsaleDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                        join prod in this.context.MasProducts on dt.PdId equals prod.PdId
                        where hd.DocStatus != "Cancel"
                            && hd.CreatedBy != "dummy"
                            && hd.DocDate == query.DocDate
                            && prod.PdType == "Product"
                            && dt.UnitBarcode != "08898"  
                        select new { hd, dt }
                         ).AsQueryable();

            result = await resp.GroupBy(x => new { x.hd.CompCode, x.hd.BrnCode, x.hd.DocDate })
                            .Select(x => new CreditsaleAmount
                            {
                                CompCode = x.Key.CompCode,
                                BrnCode = x.Key.BrnCode,
                                DocDate = x.Key.DocDate,
                                NetAmt = x.Sum(s => s.dt.SubAmt) // ขายหักส่วนลดแล้ว 
                            }).ToListAsync();

            return result;

        }

     
        public async Task AddHdAsync(SalCreditsaleHd creditSaleHd)
        {
            await context.SalCreditsaleHds.AddAsync(creditSaleHd);
        }

        public async Task AddDtAsync(SalCreditsaleDt creditSaleDt)
        {
            await context.SalCreditsaleDts.AddAsync(creditSaleDt);
        }

        public async Task AddLogAsync(SalCreditsaleLog creditSaleLog)
        {
            await context.SalCreditsaleLogs.AddAsync(creditSaleLog);
        }

        public bool CheckExistsPOS(SalCreditsaleHd obj)
        {
            return context.SalCreditsaleHds.Any(x => x.CompCode == obj.CompCode
                           && x.BrnCode == obj.BrnCode
                           && x.LocCode == obj.LocCode
                           && x.PosNo == obj.PosNo);
        }

        //public async Task<SalCreditsaleHd> UpdateRemainQuotation(SalCreditsaleHd obj)
        //{
        //    var hd = context.SalQuotationHds.FirstOrDefault(x => x.CompCode == obj.CompCode && x.BrnCode == obj.BrnCode && x.LocCode == obj.LocCode && x.DocNo == obj.RefNo);
        //    if (hd != null)
        //    {
        //        var dt = context.SalQuotationDts.Where(x => x.CompCode == obj.CompCode && x.BrnCode == obj.BrnCode && x.LocCode == obj.LocCode && x.DocNo == obj.RefNo).ToList();
        //        if (dt != null)
        //        {
        //            foreach (SalQuotationDt row in dt)
        //            {
        //                var find = obj.SalCreditsaleDt.FirstOrDefault(y => y.UnitBarcode == row.UnitBarcode); //ต้องไป 1 แถวเสมอ เพราะในเอกสารเดียวกัน จะมี Barcode สินค้าเดียวกันหลายบรรทัดไม่ได้
        //                if (obj.DocStatus == "Active")
        //                {
        //                    row.StockRemain -= (find != null ? find.StockQty : 0);
        //                }
        //            }

        //            hd.DocStatus = "Reference";
        //            context.SalQuotationHds.Update(hd);
        //            context.SalQuotationDts.UpdateRange(dt);
        //        }
        //    }
        //    return  obj;
        //}
    }
}
