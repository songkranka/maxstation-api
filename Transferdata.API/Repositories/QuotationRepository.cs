using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Helpers;
using Transferdata.API.Resources;
using Transferdata.API.Resources.Quotation;

namespace Transferdata.API.Repositories
{
    public class QuotationRepository : SqlDataAccessHelper,IQuotationRepository
    {
        public QuotationRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<SalQuotationLog> AddLogAsync(SalQuotationLog log)
        {            
            log.CreatedDate = DateTime.Now;
            this.context.SalQuotationLogs.Add(log);
            return log;
        }

        public async Task<LogResource> CancelRemainQuotation(QuotationResource obj)
        {
            var hd = context.SalQuotationHds.FirstOrDefault(x => x.BrnCode == obj.BrnCode  && x.DocNo == obj.DocNo);
            if (hd != null)
            {
                var dt = await context.SalQuotationDts.Where(x => x.CompCode == hd.CompCode && x.BrnCode == hd.BrnCode && x.LocCode == hd.LocCode && x.DocNo == hd.DocNo && x.StockRemain < x.StockQty).ToListAsync();
                if (dt != null)
                {
                    foreach (SalQuotationDt row in dt)
                    {
                        var find = obj.QuotationDt.FirstOrDefault(y => y.UnitBarcode == row.UnitBarcode); //ต้องไป 1 แถวเสมอ เพราะในเอกสารเดียวกัน จะมี Barcode สินค้าเดียวกันหลายบรรทัดไม่ได้
                        if (row.StockRemain < row.StockQty)
                        {
                            row.StockRemain += (find != null ? find.StockQty : 0);
                        }                        
                    }

                    context.SalQuotationDts.UpdateRange(dt);
                }
            }
            else
            {
                throw new Exception($"ไม่พบเอกสารเลขที่ {obj.DocNo}");
            }


            #region CreateLog
            var log = new SalQuotationLog();
            log.LogNo =   (this.context.SalQuotationLogs.Max(e => (int?)e.LogNo) ?? 0) +1;
            log.LogStatus = "Cancel";
            log.CompCode = "";
            log.BrnCode = obj.BrnCode;
            log.LocCode = "";
            log.DocNo = obj.DocNo;
            log.RefNo = obj.RefNo;
            log.JsonData = JsonConvert.SerializeObject(obj);
            log.CreatedBy = "POS";
            await this.AddLogAsync(log);
            #endregion

            LogResource res = new LogResource();
            res.LogNo = log.LogNo;
            return res;
        }

        public async Task<LogResource> CreateRemainQuotation(QuotationResource obj)
        {
            var hd = context.SalQuotationHds.FirstOrDefault(x => x.BrnCode == obj.BrnCode && x.DocNo == obj.DocNo);
            if (hd != null)
            {
                var dt = await context.SalQuotationDts.Where(x => x.CompCode == hd.CompCode && x.BrnCode == hd.BrnCode && x.LocCode == hd.LocCode && x.DocNo == hd.DocNo && x.StockRemain >0).ToListAsync();
                if (dt != null)
                {
                    foreach (SalQuotationDt row in dt)
                    {
                        var find = obj.QuotationDt.FirstOrDefault(y => y.UnitBarcode == row.UnitBarcode  ); //ต้องไป 1 แถวเสมอ เพราะในเอกสารเดียวกัน จะมี Barcode สินค้าเดียวกันหลายบรรทัดไม่ได้
                        row.StockRemain = row.StockRemain - (find != null ? find.StockQty : 0);
                    }

                    hd.DocStatus = "Reference";
                    context.SalQuotationHds.Update(hd);
                    context.SalQuotationDts.UpdateRange(dt);
                }
            }
            else
            {
                throw new Exception($"ไม่พบใบเสนอราคาเลขที่ {obj.DocNo}");
            }

            #region CreateLog
            var log = new SalQuotationLog();
            log.LogNo = (this.context.SalQuotationLogs.Max(e => (int?)e.LogNo) ?? 0) + 1;
            log.LogStatus = "Create";
            log.CompCode = "";
            log.BrnCode = obj.BrnCode;
            log.LocCode = "";
            log.DocNo = obj.DocNo;
            log.RefNo = obj.RefNo;
            log.JsonData = JsonConvert.SerializeObject(obj);
            log.CreatedBy = "POS";
            await this.AddLogAsync(log);
            #endregion


            LogResource res = new LogResource();
            res.LogNo = log.LogNo;
            return res;
        }

        public async Task<SalQuotationLog> GetLogAsync(string LogStatus, QuotationResource query)
        {
            SalQuotationLog log = new SalQuotationLog();
            log = this.context.SalQuotationLogs.FirstOrDefault(x => x.LogStatus == LogStatus
                                                                && x.BrnCode == query.BrnCode
                                                                && x.DocNo == query.DocNo
                                                                && x.RefNo == query.RefNo);
            if(log == null)
            {
                throw new Exception($"ไม่พบประวัติการสร้างจากใบเสนอราคาเลขที่ {query.DocNo} อ้างอิงเลขที่ {query.RefNo}");
            }

            return log;
        }


        public async Task<SalQuotationHd> GetQuotationAsync(QuotationResource query)
        {
            SalQuotationHd head = new SalQuotationHd();
            head = this.context.SalQuotationHds.FirstOrDefault(x => x.BrnCode == query.BrnCode && x.DocNo == query.DocNo);                    
            if(head == null)
            {
                throw new Exception($"ไม่พบเอกสารเลขที่ {query.DocNo}");
            }
            //head.SalQuotationDt = await this.context.SalQuotationDts.Where(x => x.CompCode == head.CompCode && x.BrnCode == head.BrnCode && x.LocCode == head.LocCode && x.DocNo == head.DocNo).ToListAsync();

            head.SalQuotationDt = await (from item in this.context.SalQuotationDts
                        join price in this.context.MasProductPrices on new { item.CompCode, item.BrnCode, item.PdId, item.UnitBarcode } equals new { price.CompCode, price.BrnCode, price.PdId, price.UnitBarcode }
                        where item.CompCode == head.CompCode &&
                        item.BrnCode == head.BrnCode &&
                        item.LocCode == head.LocCode &&
                        item.DocNo == head.DocNo
                        select new { item, price }
                        ).Select(x => new SalQuotationDt {
                            CompCode = x.item.CompCode,
                            BrnCode = x.item.BrnCode,
                            LocCode = x.item.LocCode,
                            DocNo = x.item.DocNo,
                            SeqNo = x.item.SeqNo,
                            IsFree = x.item.IsFree,
                            PdId = x.item.PdId,
                            PdName = x.item.PdName,
                            UnitId = x.item.UnitId,
                            UnitName = x.item.UnitName,
                            UnitBarcode = x.item.UnitBarcode,
                            ItemQty = x.item.ItemQty,
                            StockQty = x.item.StockQty,
                            StockRemain = x.item.StockRemain,
                            RefPrice = x.price.Unitprice,
                            RefPriceCur = x.price.Unitprice,
                            UnitPrice = x.item.UnitPrice,
                            UnitPriceCur = x.item.UnitPriceCur,
                            SumItemAmt = x.item.SumItemAmt,
                            SumItemAmtCur = x.item.SumItemAmtCur,
                            DiscAmt = x.item.DiscAmt,
                            DiscAmtCur = x.item.DiscAmtCur,
                            DiscHdAmt = x.item.DiscHdAmt,
                            DiscHdAmtCur = x.item.DiscHdAmtCur,
                            SubAmt = x.item.SubAmt,
                            SubAmtCur = x.item.SubAmtCur,
                            VatType = x.item.VatType,
                            VatRate = x.item.VatRate,
                            VatAmt = x.item.VatAmt,
                            VatAmtCur = x.item.VatAmtCur,
                            TaxBaseAmt = x.item.TaxBaseAmt,
                            TaxBaseAmtCur = x.item.TaxBaseAmtCur,
                            TotalAmt = x.item.TotalAmt,
                            TotalAmtCur = x.item.TotalAmtCur                          
                        }).ToListAsync();

            return head;
        }

        public async Task<List<QuotationMaxCardResource>> ListByMaxCardAsync(QuotationResource query)
        {
            List<QuotationMaxCardResource> heads = new List<QuotationMaxCardResource>();
            heads = await (from head in this.context.SalQuotationHds
                               join item in this.context.SalQuotationDts on new { head.CompCode, head.BrnCode, head.LocCode, head.DocNo } equals new { item.CompCode, item.BrnCode, item.LocCode, item.DocNo }
                               where (head.DocStatus == "Ready" || head.DocStatus == "Reference")
                               && head.BrnCode == query.BrnCode
                               && head.MaxCardId == query.MaxCardId
                               && item.StockRemain > 0
                               select new { head,item }
                        ).GroupBy(x => new { x.head.CompCode, x.head.BrnCode, x.head.LocCode, x.head.DocNo, x.head.DocDate, x.head.CustName, x.head.MaxCardId }).Select(x => new QuotationMaxCardResource
                        {
                            BrnCode = x.Key.BrnCode,
                            DocNo = x.Key.DocNo,
                            DocDate = x.Key.DocDate,                            
                            CustName = x.Key.CustName,
                            MaxCardId = x.Key.MaxCardId,
                            ItemCount = x.Count()
                        }).OrderBy(x=>x.DocNo).ToListAsync();

            return heads;
        }

        public async Task<SalQuotationHd> UpdateRemainQuotation(QuotationResource obj)
        {
            var hd =  context.SalQuotationHds.FirstOrDefault(x => x.BrnCode == obj.BrnCode && x.DocNo == obj.DocNo);
            if (hd != null)
            {
                var dt = await context.SalQuotationDts.Where(x => x.CompCode == hd.CompCode && x.BrnCode == hd.BrnCode && x.LocCode == hd.LocCode && x.DocNo == hd.DocNo).ToListAsync();
                if (dt != null)
                {
                    foreach (SalQuotationDt row in dt)
                    {
                        var find = obj.QuotationDt.FirstOrDefault(y => y.UnitBarcode == row.UnitBarcode); //ต้องไป 1 แถวเสมอ เพราะในเอกสารเดียวกัน จะมี Barcode สินค้าเดียวกันหลายบรรทัดไม่ได้
                        row.StockRemain = (row.StockRemain + find.ItemQtyBefore) -  (find != null ? find.StockQty : 0);
                    }

                    //hd.DocStatus = "Reference";
                    //context.SalQuotationHds.Update(hd);
                    context.SalQuotationDts.UpdateRange(dt);
                }
            }
            return hd;
        }

       

    }
}
