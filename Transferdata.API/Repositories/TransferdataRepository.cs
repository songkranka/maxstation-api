using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Helpers;
using Transferdata.API.Resources;
using Transferdata.API.Resources.Transferdata;

namespace Transferdata.API.Repositories
{
    public class TransferdataRepository : SqlDataAccessHelper, ITransferdataRepository
    {
        public TransferdataRepository(PTMaxstationContext context) : base(context)
        {

        }


        public async Task<DopPostdayLog> AddLogAsync(DopPostdayLog log)
        {
            log.CreatedDate = DateTime.Now;
            this.context.DopPostdayLogs.Add(log);
            return log;
        }

        //Task<LogResource> ITransferdataRepository.UpdateCloseDayAsync(TransferDataResource query)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task UpdateCloseDayAsync(TransferDataResource query)
        public async Task<LogResource> UpdateCloseDayAsync(TransferDataResource query)
        {
            #region Period
            var dopPeriod = await context.DopPeriods
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            dopPeriod.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(dopPeriod);
            context.SaveChanges();
            #endregion

            #region Request
            var invRequestHds = await context.InvRequestHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            invRequestHds.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(invRequestHds);
            #endregion

            #region ReceiveProd
            var invReceiveProd = await context.InvReceiveProdHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            invReceiveProd.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(invReceiveProd);
            #endregion

            #region TransOut
            var invTransOut = await context.InvTranoutHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            invTransOut.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(invTransOut);
            #endregion

            #region TransIn
            var invTransIn = await context.InvTraninHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            invTransIn.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(invTransIn);
            #endregion

            #region Withdraw
            var invWithdraw = await context.InvWithdrawHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            invWithdraw.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(invWithdraw);
            #endregion

            #region ReturnSup
            var invReturnSup = await context.InvReturnSupHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            invReturnSup.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(invReturnSup);
            #endregion

            #region ReturnOil
            var invReturnOil = await context.InvReturnOilHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            invReturnOil.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(invReturnOil);
            #endregion

            #region Unusable
            var invUnusable = await context.InvUnuseHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            invUnusable.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(invUnusable);
            #endregion


            #region Quotation
            var salQuotationHds = await context.SalQuotationHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            salQuotationHds.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(salQuotationHds);
            #endregion

            #region CashSale
            var salCashHds = await context.SalCashsaleHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            salCashHds.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });
            context.UpdateRange(salCashHds);
            #endregion

            #region CreditSale
            var salCreditHds = await context.SalCreditsaleHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            salCreditHds.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(salCreditHds);            
            #endregion

            #region Taxinvoice
            var salTaxinvoiceHds = await context.SalTaxinvoiceHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            salTaxinvoiceHds.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });
            context.UpdateRange(salTaxinvoiceHds);
            #endregion

            #region CN/DN
            var salCndnHd = await context.SalCndnHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            salCndnHd.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });
            context.UpdateRange(salCndnHd);
            #endregion

            #region Billing
            var salBilling = await context.SalBillingHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            salBilling.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });
            context.UpdateRange(salBilling);
            #endregion

            #region FinanceReceive
            var finReceive = await context.FinReceiveHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate && x.Post != "P"
                ).ToListAsync();

            finReceive.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });
            context.UpdateRange(finReceive);
            #endregion


            #region mas_control
            var icControl = await context.MasControls.Where(x => x.CompCode == query.CompCode
                                                        && x.BrnCode == query.BrnCode
                                                        && x.CtrlCode == "WDATE").ToListAsync();
          
            if(icControl == null || icControl.Count() == 0)
            {
                MasControl ctl = new MasControl
                {
                    CompCode = query.CompCode,
                    BrnCode = query.BrnCode,
                    LocCode = "000",
                    CtrlCode = "WDATE",
                    CtrlValue = query.WDate.Value.AddDays(1).ToString("dd/MM/yyyy"),
                    Remark = "Work Date",
                    CreatedBy = query.User,
                    CreatedDate = DateTime.Now
                };
                context.Add(ctl);
            }
            else
            {
                icControl.ForEach(x => {
                    x.CtrlValue = query.WDate.Value.AddDays(1).ToString("dd/MM/yyyy");  //ขยับวันทำงานไป 1 วัน
                    x.UpdatedDate = DateTime.Now;
                    x.UpdatedBy = query.User;
                });
                context.UpdateRange(icControl);
            }

            #endregion

            #region Summary CreditSaleHd to TaxInvoice
            this.context.SaveChanges();
            await CreateTaxInvoiceAsync(query);
            #endregion


            #region CreateLog
            //query.CreateDate = DateTime.Now.AddHours(7);
            //var log = new DopPostdayLog();
            //log.CompCode = query.CompCode;
            //log.BrnCode = query.BrnCode;
            //log.LocCode = "";
            //log.DocNo = "";
            //log.JsonData = JsonConvert.SerializeObject(query);
            //log.CreatedBy = query.User;
            //await this.AddLogAsync(log);
            #endregion

            LogResource res = new LogResource();
            //res.LogNo = log.LogNo;
            return res;
        }



        public async Task CreateTaxInvoiceAsync(TransferDataResource query)
        {
            #region DocPattern
            var docpattern = this.context.MasDocPatterns.FirstOrDefault(x => x.DocType == "TaxInvoice");
            var pattern = (docpattern == null) ? "" : docpattern.Pattern;
            pattern = pattern.Replace("Brn", query.BrnCode);
            pattern = pattern.Replace("yy", query.WDate.Value.ToString("yy"));
            pattern = pattern.Replace("MM", query.WDate.Value.ToString("MM"));

            var runno = GetRunNumber(query.CompCode, query.BrnCode, pattern);
            SalTaxinvoiceHd taxinvoicehd = new SalTaxinvoiceHd();
            #endregion


            #region CreditSale N:1
            //เช็คการรวบบิลตามลูกค้า
            //หาก่อนว่า วันนี้มีลูกค้ากี่ราย
            var creditsales = await context.SalCreditsaleHds.Where(x => x.CompCode == query.CompCode
                                                            && x.BrnCode == query.BrnCode
                                                            && x.Post == "P"
                                                            && x.DocStatus != "Cancel"
                                                            && x.DocType == "CreditSale"
                                                            && (x.TxNo == "" || x.TxNo == null))
                                        .GroupBy(x => new { x.CompCode, x.BrnCode, x.LocCode, x.DocType, x.DocDate, x.CustCode, x.CustName })
                                        .Select(x => new SalCreditsaleHd
                                        {
                                            CompCode = x.Key.CompCode,
                                            BrnCode = x.Key.BrnCode,
                                            LocCode = x.Key.LocCode,
                                            DocType = x.Key.DocType,
                                            DocDate = x.Key.DocDate,
                                            CustCode = x.Key.CustCode,
                                            CustName = x.Key.CustName,
                                        }).ToListAsync();

            foreach (var creditsalehd in creditsales) //list creditsale group by cust
            {
                var cust = this.context.MasCustomers.Find(creditsalehd.CustCode);
                if (cust.BillType == "Y")
                {
                    //รวบบิล => เอา item ทั้งหมด ออกมาแล้วรวมกัน
                    //var customer = this.context.MasCustomers.Find(cust.CustCode);

                    List<string> docnos = this.context.SalCreditsaleHds.Where(x => x.DocStatus != "Cancel" && x.Post == "P"
                                                                        && (x.TxNo == null || x.TxNo == "")
                                                                        && x.CompCode == creditsalehd.CompCode
                                                                        && x.BrnCode == creditsalehd.BrnCode
                                                                        && x.LocCode == creditsalehd.LocCode
                                                                        && x.DocType == creditsalehd.DocType
                                                                        && x.DocDate == creditsalehd.DocDate
                                                                        && x.CustCode == creditsalehd.CustCode
                                                                        ).Select(x => x.DocNo).ToList();

                    var salehd = this.context.SalCreditsaleHds.Where(x => x.CompCode == creditsalehd.CompCode
                                                                    && x.BrnCode == creditsalehd.BrnCode
                                                                    && x.LocCode == creditsalehd.LocCode
                                                                    && x.DocType == creditsalehd.DocType
                                                                    && docnos.Contains(x.DocNo)
                                ).GroupBy(x => new { x.CompCode, x.BrnCode, x.LocCode, x.DocType, x.DocDate, x.CustCode, x.Currency, x.CurRate })
                                .Select(x => new SalCreditsaleHd
                                {
                                    CompCode = x.Key.CompCode,
                                    BrnCode = x.Key.BrnCode,
                                    LocCode = x.Key.LocCode,
                                    DocType = x.Key.DocType,
                                    DocDate = x.Key.DocDate,
                                    CustCode = x.Key.CustCode,
                                    Currency = x.Key.Currency,
                                    CurRate = x.Key.CurRate,
                                    SubAmt = x.Sum(s => Math.Round((decimal)s.SubAmt, 2)),
                                    SubAmtCur = x.Sum(s => Math.Round((decimal)s.SubAmtCur, 2)),
                                    DiscAmt = x.Sum(s => s.DiscAmt),
                                    DiscAmtCur = x.Sum(s => s.DiscAmtCur),
                                }).FirstOrDefault();



                    var items = await this.context.SalCreditsaleDts.Where(d => d.CompCode == creditsalehd.CompCode
                                                        && d.BrnCode == creditsalehd.BrnCode
                                                        && d.LocCode == creditsalehd.LocCode
                                                        && d.DocType == creditsalehd.DocType
                                                        && docnos.Contains(d.DocNo)
                                                        ).GroupBy(x => new
                                                        {
                                                            x.CompCode,
                                                            x.BrnCode,
                                                            x.LocCode,
                                                            x.DocType,
                                                            x.PdId,
                                                            x.PdName,
                                                            x.UnitId,
                                                            x.UnitName,
                                                            x.UnitBarcode,
                                                            x.IsFree,
                                                            x.UnitPrice,
                                                            x.UnitPriceCur,
                                                            x.VatType,
                                                            x.VatRate
                                                        })
                                                        .Select(x => new SalTaxinvoiceDt
                                                        {
                                                            CompCode = x.Key.CompCode,
                                                            BrnCode = x.Key.BrnCode,
                                                            LocCode = x.Key.LocCode,
                                                            PdId = x.Key.PdId,
                                                            PdName = x.Key.PdName,
                                                            UnitId = x.Key.UnitId,
                                                            UnitName = x.Key.UnitName,
                                                            UnitBarcode = x.Key.UnitBarcode,
                                                            IsFree = x.Key.IsFree,
                                                            UnitPrice = x.Key.UnitPrice,
                                                            UnitPriceCur = x.Key.UnitPriceCur,
                                                            VatType = x.Key.VatType,
                                                            VatRate = x.Key.VatRate,                                                            
                                                            ItemQty = x.Sum(s => s.ItemQty),
                                                            StockQty = x.Sum(s => s.StockQty),
                                                            SumItemAmt = x.Sum(s => s.SumItemAmt),
                                                            SumItemAmtCur = x.Sum(s => s.SumItemAmtCur),
                                                            DiscAmt = x.Sum(s => s.DiscAmt),
                                                            DiscAmtCur = x.Sum(s => s.DiscAmtCur),
                                                            DiscHdAmt = x.Sum(s => s.DiscHdAmt),
                                                            DiscHdAmtCur = x.Sum(s => s.DiscHdAmtCur),
                                                            TaxBaseAmt = x.Sum(s => s.TaxBaseAmt),
                                                            TaxBaseAmtCur = x.Sum(s => s.TaxBaseAmtCur),
                                                            VatAmt = x.Sum(s => s.VatAmt),
                                                            VatAmtCur = x.Sum(s => s.VatAmtCur),
                                                            TotalAmt = x.Sum(s => s.TotalAmt),
                                                            TotalAmtCur = x.Sum(s => s.TotalAmtCur),
                                                        }).ToListAsync();


                    taxinvoicehd.RunNumber = ++runno;
                    taxinvoicehd.CompCode = salehd.CompCode;
                    taxinvoicehd.BrnCode = salehd.BrnCode;
                    taxinvoicehd.LocCode = salehd.LocCode;
                    taxinvoicehd.DocDate = salehd.DocDate;
                    taxinvoicehd.DocNo = GenDocNo(query, pattern, (int)taxinvoicehd.RunNumber);
                    taxinvoicehd.DocStatus = "Active";
                    taxinvoicehd.DocType = "CreditSale";
                    taxinvoicehd.CustCode = salehd.CustCode;
                    taxinvoicehd.CustName = $"{cust.CustPrefix} {cust.CustName}";
                    taxinvoicehd.CustAddr1 = cust.CustAddr1;
                    taxinvoicehd.CustAddr2 = cust.CustAddr2;
                    taxinvoicehd.CitizenId = cust.CitizenId ?? "";
                    taxinvoicehd.Currency = salehd.Currency;
                    taxinvoicehd.CurRate = salehd.CurRate;
                    taxinvoicehd.DiscRate = "";
                    taxinvoicehd.DiscAmt = Math.Round((decimal)salehd.DiscAmt, 2);
                    taxinvoicehd.DiscAmtCur = Math.Round((decimal)salehd.DiscAmtCur, 2);


                    int seqno = 0;
                    decimal? subamt = 0;
                    decimal? subamtcur = 0;

                    items.ForEach(x => {
                        x.SubAmt = x.SumItemAmt - x.DiscAmt;
                        x.SubAmtCur = x.SumItemAmtCur - x.DiscAmtCur;
                        subamt += x.SubAmt;
                        subamtcur += x.SubAmtCur;
                    });
                    taxinvoicehd.SubAmt = Math.Round((decimal)subamt, 2);
                    taxinvoicehd.SubAmtCur = Math.Round((decimal)subamtcur, 2);

                    foreach (var item in items)
                    {
                        item.DocNo = taxinvoicehd.DocNo;
                        item.SeqNo = ++seqno;

                        var licenseplates = this.context.SalCreditsaleDts.Where(d => d.CompCode == creditsalehd.CompCode
                                                           && d.BrnCode == creditsalehd.BrnCode
                                                           && d.LocCode == creditsalehd.LocCode
                                                           && d.DocType == creditsalehd.DocType
                                                           && d.PdId == item.PdId
                                                           && d.UnitBarcode == item.UnitBarcode
                                                           && docnos.Contains(d.DocNo)).Select(x => x.LicensePlate).Distinct().Take(10).ToList();    
                        item.LicensePlate = string.Join(",", licenseplates);
                        this.context.SalTaxinvoiceDts.Add(item);                  

                    }//items



                    taxinvoicehd.TotalAmt = taxinvoicehd.SubAmt - taxinvoicehd.DiscAmt;  //totalamt;
                    taxinvoicehd.TotalAmtCur = taxinvoicehd.SubAmtCur - taxinvoicehd.DiscAmtCur; // totalamtcur;
                    taxinvoicehd.TaxBaseAmt = items.Sum(x => x.TaxBaseAmt);
                    taxinvoicehd.TaxBaseAmtCur = items.Sum(x => x.TaxBaseAmtCur);
                    taxinvoicehd.VatRate = 0;
                    taxinvoicehd.VatAmt = items.Sum(x => x.VatAmt);
                    taxinvoicehd.VatAmtCur = items.Sum(x => x.VatAmtCur);
                    taxinvoicehd.NetAmt = taxinvoicehd.TaxBaseAmt + taxinvoicehd.VatAmt;
                    taxinvoicehd.NetAmtCur = taxinvoicehd.TaxBaseAmtCur + taxinvoicehd.VatAmtCur;

                    taxinvoicehd.ItemCount = items.Count();
                    taxinvoicehd.Post = "P";
                    taxinvoicehd.DocPattern = pattern;
                    taxinvoicehd.Guid = Guid.NewGuid();
                    taxinvoicehd.CreatedBy = query.User;
                    taxinvoicehd.CreatedDate = DateTime.Now;
                    this.context.SalTaxinvoiceHds.Add(taxinvoicehd);

                    //insert balance
                    var balance = new FinBalance
                    {
                        CompCode = taxinvoicehd.CompCode,
                        BrnCode = taxinvoicehd.BrnCode,
                        LocCode = taxinvoicehd.LocCode,
                        DocType = taxinvoicehd.DocType,
                        DocNo = taxinvoicehd.DocNo,
                        DocDate = taxinvoicehd.DocDate,
                        CustCode = taxinvoicehd.CustCode,
                        Currency = taxinvoicehd.Currency,
                        NetAmt = Math.Round((decimal)taxinvoicehd.NetAmt, 2),
                        NetAmtCur = Math.Round((decimal)taxinvoicehd.NetAmtCur, 2),
                        BalanceAmt = Math.Round((decimal)taxinvoicehd.NetAmt, 2), //0,
                        BalanceAmtCur = Math.Round((decimal)taxinvoicehd.NetAmtCur, 2), //0,
                        CreatedBy = query.User,
                        CreatedDate = DateTime.Now
                    };
                    this.context.FinBalances.Add(balance);

                    // update creditsalehd
                    var sales = this.context.SalCreditsaleHds.Where(x => x.CompCode == creditsalehd.CompCode
                                                                        && x.BrnCode == creditsalehd.BrnCode
                                                                        && x.LocCode == creditsalehd.LocCode
                                                                        && x.DocType == creditsalehd.DocType
                                                                        && docnos.Contains(x.DocNo)
                                                                        ).ToList();
                    sales.ForEach(x =>
                    {
                        x.DocStatus = "Reference";
                        x.TxNo = taxinvoicehd.DocNo;
                        x.UpdatedBy = query.User;
                        x.UpdatedDate = DateTime.Now;
                    });
                    this.context.SalCreditsaleHds.UpdateRange(sales);
                    this.context.SaveChanges();

                    //end รวบบิล  
                }
                else
                {
                    //ไม่ต้องรวบบิล  => ดึงทุกใบของลูกค้านี้ขึ้นมา
                    var salehds = await this.context.SalCreditsaleHds.Where(x => x.DocStatus != "Cancel" && x.Post == "P"
                                                   && (x.TxNo == null || x.TxNo == "")
                                                   && x.CompCode == creditsalehd.CompCode
                                                   && x.BrnCode == creditsalehd.BrnCode
                                                   && x.LocCode == creditsalehd.LocCode
                                                   && x.DocType == creditsalehd.DocType
                                                   && x.DocDate == creditsalehd.DocDate
                                                   && x.CustCode == creditsalehd.CustCode
                                                   ).OrderBy(x => x.DocNo).ToListAsync();

                    foreach (var salehd in salehds)
                    {
                        //insert taxinvoice hd
                        taxinvoicehd = new SalTaxinvoiceHd();
                        taxinvoicehd.RunNumber = ++runno;
                        taxinvoicehd.CompCode = salehd.CompCode;
                        taxinvoicehd.BrnCode = salehd.BrnCode;
                        taxinvoicehd.LocCode = salehd.LocCode;
                        taxinvoicehd.DocNo = GenDocNo(query, pattern, (int)taxinvoicehd.RunNumber);
                        taxinvoicehd.DocStatus = "Active";
                        taxinvoicehd.DocType = "CreditSale";
                        taxinvoicehd.DocDate = salehd.DocDate;
                        taxinvoicehd.CustCode = salehd.CustCode;
                        taxinvoicehd.CustName = $"{cust.CustPrefix} {cust.CustName}";
                        taxinvoicehd.CitizenId = cust.CitizenId;
                        taxinvoicehd.CustAddr1 = cust.CustAddr1; //salehd.CustAddr1;
                        taxinvoicehd.CustAddr2 = cust.CustAddr2; //salehd.CustAddr2;
                        taxinvoicehd.RefNo = salehd.RefNo;
                        taxinvoicehd.ItemCount = salehd.ItemCount;
                        taxinvoicehd.Currency = salehd.Currency;
                        taxinvoicehd.CurRate = salehd.CurRate;
                        taxinvoicehd.SubAmt = salehd.SubAmt;
                        taxinvoicehd.SubAmtCur = salehd.SubAmtCur;
                        taxinvoicehd.DiscRate = salehd.DiscRate;
                        taxinvoicehd.DiscAmt = salehd.DiscAmt;
                        taxinvoicehd.DiscAmtCur = salehd.DiscAmtCur;
                        taxinvoicehd.TotalAmt = Math.Round((decimal)salehd.TotalAmt, 2);
                        taxinvoicehd.TotalAmtCur = Math.Round((decimal)salehd.TotalAmtCur, 2);
                        taxinvoicehd.TaxBaseAmt = Math.Round((decimal)salehd.TaxBaseAmt, 2);
                        taxinvoicehd.TaxBaseAmtCur = Math.Round((decimal)salehd.TaxBaseAmtCur, 2);
                        taxinvoicehd.VatRate = salehd.VatRate;
                        taxinvoicehd.VatAmt = Math.Round((decimal)salehd.VatAmt, 2);
                        taxinvoicehd.VatAmtCur = Math.Round((decimal)salehd.VatAmtCur, 2);
                        taxinvoicehd.NetAmt = Math.Round((decimal)salehd.NetAmt, 2);
                        taxinvoicehd.NetAmtCur = Math.Round((decimal)salehd.NetAmtCur, 2);
                        taxinvoicehd.Post = "P";
                        taxinvoicehd.DocPattern = pattern;


                        //insert taxinvoice dt
                        var items = this.context.SalCreditsaleDts.Where(x => x.CompCode == salehd.CompCode
                                                                       && x.BrnCode == salehd.BrnCode
                                                                       && x.LocCode == salehd.LocCode
                                                                       && x.DocType == salehd.DocType
                                                                       && x.DocNo == salehd.DocNo)
                                                                .Select(x => new SalTaxinvoiceDt
                                                                {
                                                                    CompCode = x.CompCode,
                                                                    BrnCode = x.BrnCode,
                                                                    LocCode = x.LocCode,
                                                                    DocNo = taxinvoicehd.DocNo,
                                                                    SeqNo = x.SeqNo,
                                                                    LicensePlate = x.LicensePlate,
                                                                    PdId = x.PdId,
                                                                    PdName = x.PdName,
                                                                    IsFree = x.IsFree,
                                                                    UnitId = x.UnitId,
                                                                    UnitBarcode = x.UnitBarcode,
                                                                    UnitName = x.UnitName,
                                                                    ItemQty = x.ItemQty??decimal.Zero,
                                                                    StockQty = x.StockQty??decimal.Zero,
                                                                    UnitPrice = x.UnitPrice,
                                                                    UnitPriceCur = x.UnitPriceCur,
                                                                    SumItemAmt = x.SumItemAmt,
                                                                    SumItemAmtCur = x.SumItemAmtCur,
                                                                    DiscAmt = x.DiscAmt,
                                                                    DiscAmtCur = x.DiscAmtCur,
                                                                    DiscHdAmt = Math.Round((decimal)x.DiscHdAmt, 2),
                                                                    DiscHdAmtCur = Math.Round((decimal)x.DiscHdAmtCur, 2),
                                                                    SubAmt = x.SubAmt,
                                                                    SubAmtCur = x.SubAmtCur,
                                                                    VatType = x.VatType,
                                                                    VatRate = x.VatRate,
                                                                    VatAmt = Math.Round((decimal)x.VatAmt, 2),
                                                                    VatAmtCur = Math.Round((decimal)x.VatAmtCur, 2),
                                                                    TaxBaseAmt = Math.Round((decimal)x.TaxBaseAmt, 2),
                                                                    TaxBaseAmtCur = Math.Round((decimal)x.TaxBaseAmtCur, 2),
                                                                    TotalAmt = Math.Round((decimal)x.TotalAmt, 2),
                                                                    TotalAmtCur = Math.Round((decimal)x.TotalAmtCur, 2)
                                                                }).ToList();

                        this.context.SalTaxinvoiceDts.AddRange(items);

                        taxinvoicehd.ItemCount = items.Count();
                        taxinvoicehd.Guid = Guid.NewGuid();
                        taxinvoicehd.CreatedBy = query.User;
                        taxinvoicehd.CreatedDate = DateTime.Now;

                        this.context.SalTaxinvoiceHds.Add(taxinvoicehd);                        

                        //insert balance
                        var balance = new FinBalance
                        {
                            CompCode = taxinvoicehd.CompCode,
                            BrnCode = taxinvoicehd.BrnCode,
                            LocCode = taxinvoicehd.LocCode,
                            DocType = taxinvoicehd.DocType,
                            DocNo = taxinvoicehd.DocNo,
                            DocDate = taxinvoicehd.DocDate,
                            CustCode = taxinvoicehd.CustCode,
                            Currency = taxinvoicehd.Currency,
                            NetAmt = Math.Round((decimal)taxinvoicehd.NetAmt, 2),
                            NetAmtCur = Math.Round((decimal)taxinvoicehd.NetAmtCur, 2),
                            BalanceAmt = Math.Round((decimal)taxinvoicehd.NetAmt, 2), // 0,
                            BalanceAmtCur = Math.Round((decimal)taxinvoicehd.NetAmtCur, 2), // 0,
                            CreatedBy = query.User,
                            CreatedDate = DateTime.Now
                        };
                        this.context.FinBalances.Add(balance);


                        // update creditsalehd

                        salehd.DocStatus = "Reference";
                        salehd.TxNo = taxinvoicehd.DocNo;  // reference tax_doc_no
                        salehd.UpdatedBy = query.User;
                        salehd.UpdatedDate = DateTime.Now;
                        this.context.SalCreditsaleHds.Update(salehd);
                        this.context.SaveChanges();

                    }//foreach salehd


                }//BillType


            }//foreach
            #endregion


            #region Invoice 1:1 
            //ไม่ต้องรวบเอกสาร
            var taxinvs = await context.SalCreditsaleHds.Where(x => x.CompCode == query.CompCode
                                                            && x.BrnCode == query.BrnCode
                                                            && x.Post == "P"
                                                            && x.DocType == "Invoice"
                                                            && (x.TxNo == "" || x.TxNo == null))
                                        .Select(x => new SalTaxinvoiceHd
                                        {
                                            CompCode = x.CompCode,
                                            BrnCode = x.BrnCode,
                                            LocCode = x.LocCode,
                                            DocType = x.DocType,
                                            DocNo = x.DocNo,
                                            DocDate = x.DocDate,
                                            CustCode = x.CustCode,
                                            CustName = x.CustName,
                                            CustAddr1 = x.CustAddr1,
                                            CustAddr2 = x.CustAddr2,
                                            Currency = x.Currency,
                                            CurRate = x.CurRate,
                                            ItemCount = x.ItemCount,
                                            SubAmt = x.SubAmt,
                                            SubAmtCur = x.SubAmtCur,
                                            DiscRate = x.DiscRate,
                                            DiscAmt = x.DiscAmt,
                                            DiscAmtCur = x.DiscAmtCur,
                                            TotalAmt = x.TotalAmt,
                                            TotalAmtCur = x.TotalAmtCur,
                                            TaxBaseAmt = Math.Round((decimal)x.TaxBaseAmt, 2),
                                            TaxBaseAmtCur = Math.Round((decimal)x.TaxBaseAmtCur, 2),
                                            VatRate = x.VatRate,
                                            VatAmt = Math.Round((decimal)x.VatAmt, 2),
                                            VatAmtCur = Math.Round((decimal)x.VatAmtCur, 2),
                                            NetAmt = Math.Round((decimal)x.NetAmt, 2),
                                            NetAmtCur = Math.Round((decimal)x.NetAmtCur, 2),
                                            Post = x.Post
                                        }).ToListAsync();


            foreach (var taxinvhd in taxinvs)
            {
                var invno = taxinvhd.DocNo;
                taxinvhd.RunNumber = ++runno;
                taxinvhd.DocNo = GenDocNo(query, pattern, (int)taxinvhd.RunNumber);
                taxinvhd.DocStatus = "Active";
                taxinvhd.DocPattern = pattern;
                var cust = this.context.MasCustomers.Find(taxinvhd.CustCode);
                taxinvhd.CustName = cust.CustPrefix + " " + cust.CustName;
                taxinvhd.CustAddr1 = cust.CustAddr1;
                taxinvhd.CustAddr2 = cust.CustAddr2;
                taxinvhd.CitizenId = cust.CitizenId;

                var items = this.context.SalCreditsaleDts.Where(x => x.CompCode == taxinvhd.CompCode
                                                       && x.BrnCode == taxinvhd.BrnCode
                                                       && x.LocCode == taxinvhd.LocCode
                                                       && x.DocType == taxinvhd.DocType
                                                       && x.DocNo == invno)
                                                        .Select(x => new SalTaxinvoiceDt
                                                        {
                                                            CompCode = x.CompCode,
                                                            BrnCode = x.BrnCode,
                                                            LocCode = x.LocCode,
                                                            DocNo = taxinvhd.DocNo,
                                                            SeqNo = x.SeqNo,
                                                            LicensePlate = "",
                                                            PdId = x.PdId,
                                                            PdName = x.PdName,
                                                            IsFree = x.IsFree,
                                                            UnitId = x.UnitId,
                                                            UnitBarcode = x.UnitBarcode,
                                                            UnitName = x.UnitName,
                                                            ItemQty = x.ItemQty??decimal.Zero,
                                                            StockQty = x.StockQty??decimal.Zero,
                                                            UnitPrice = x.UnitPrice,
                                                            UnitPriceCur = x.UnitPriceCur,
                                                            SumItemAmt = x.SumItemAmt,
                                                            SumItemAmtCur = x.SumItemAmtCur,
                                                            DiscAmt = x.DiscAmt,
                                                            DiscAmtCur = x.DiscAmtCur,
                                                            DiscHdAmt = Math.Round((decimal)x.DiscHdAmt, 2),
                                                            DiscHdAmtCur = Math.Round((decimal)x.DiscHdAmtCur, 2),
                                                            SubAmt = x.SubAmt,
                                                            SubAmtCur = x.SubAmtCur,
                                                            VatType = x.VatType,
                                                            VatRate = x.VatRate,
                                                            VatAmt = Math.Round((decimal)x.VatAmt, 2),
                                                            VatAmtCur = Math.Round((decimal)x.VatAmtCur, 2),
                                                            TaxBaseAmt = Math.Round((decimal)x.TaxBaseAmt, 2),
                                                            TaxBaseAmtCur = Math.Round((decimal)x.TaxBaseAmtCur, 2),
                                                            TotalAmt = Math.Round((decimal)x.TotalAmt, 2),
                                                            TotalAmtCur = Math.Round((decimal)x.TotalAmtCur, 2)
                                                        }).ToList();

                this.context.SalTaxinvoiceDts.AddRange(items);

                taxinvhd.ItemCount = items.Count;
                taxinvhd.CreatedBy = query.User;
                taxinvhd.CreatedDate = DateTime.Now;
                this.context.SalTaxinvoiceHds.Add(taxinvhd);

                var balance = new FinBalance
                {
                    CompCode = taxinvhd.CompCode,
                    BrnCode = taxinvhd.BrnCode,
                    LocCode = taxinvhd.LocCode,
                    DocType = taxinvhd.DocType,
                    DocNo = taxinvhd.DocNo,
                    DocDate = taxinvhd.DocDate,
                    CustCode = taxinvhd.CustCode,
                    Currency = taxinvhd.Currency,
                    NetAmt = Math.Round((decimal)taxinvhd.NetAmt, 2),
                    NetAmtCur = Math.Round((decimal)taxinvhd.NetAmtCur, 2),
                    BalanceAmt = Math.Round((decimal)taxinvhd.NetAmt, 2), //0,
                    BalanceAmtCur = Math.Round((decimal)taxinvhd.NetAmtCur, 2), //0,
                    CreatedBy = query.User,
                    CreatedDate = DateTime.Now
                };
                this.context.FinBalances.Add(balance);

                //update salehd
                var salehds = await this.context.SalCreditsaleHds.Where(x => (x.TxNo == null || x.TxNo == "")
                                                && x.Post == taxinvhd.Post
                                                && x.CompCode == taxinvhd.CompCode
                                                && x.BrnCode == taxinvhd.BrnCode
                                                && x.LocCode == taxinvhd.LocCode
                                                && x.DocType == taxinvhd.DocType
                                                && x.DocNo == invno
                                                ).ToListAsync();
                salehds.ForEach(x =>
                {
                    x.DocStatus = "Reference";
                    x.TxNo = taxinvhd.DocNo;
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                });
                this.context.SalCreditsaleHds.UpdateRange(salehds);
                this.context.SaveChanges();

            }//end foreach
            #endregion


        }


        //public async Task CreateTaxInvoiceAsync(TransferDataResource query)
        //{
        //    #region DocPattern
        //    var docpattern = this.context.MasDocPatterns.FirstOrDefault(x => x.DocType == "TaxInvoice");
        //    var pattern = (docpattern == null) ? "" : docpattern.Pattern;
        //    pattern = pattern.Replace("Brn", query.BrnCode);
        //    pattern = pattern.Replace("yy", query.WDate.Value.ToString("yy"));
        //    pattern = pattern.Replace("MM", query.WDate.Value.ToString("MM"));

        //    var runno = GetRunNumber(pattern);
        //    #endregion

        //    var taxinvs = await context.SalCreditsaleHds.Where(x => x.CompCode == query.CompCode
        //                                                    && x.BrnCode == query.BrnCode
        //                                                    && x.Post == "P"
        //                                                    && (x.TxNo == "" || x.TxNo == null))
        //                                //.GroupBy(x => new{x.CompCode,x.BrnCode,x.LocCode,x.DocType,x.DocDate,x.CustCode,x.Post})
        //                                .Select(x => new SalTaxinvoiceHd
        //                                {
        //                                    CompCode = x.CompCode,
        //                                    BrnCode = x.BrnCode,
        //                                    LocCode = x.LocCode,
        //                                    DocType = x.DocType,
        //                                    DocNo = x.DocNo,
        //                                    DocDate = x.DocDate,
        //                                    CustCode = x.CustCode,
        //                                    CustName = x.CustName,
        //                                    CustAddr1 = x.CustAddr1,
        //                                    CustAddr2 = x.CustAddr2,
        //                                    Currency = x.Currency,
        //                                    CurRate = x.CurRate,
        //                                    ItemCount = x.ItemCount,
        //                                    SubAmt = x.SubAmt,
        //                                    SubAmtCur = x.SubAmtCur,
        //                                    DiscRate = x.DiscRate,
        //                                    DiscAmt = x.DiscAmt,
        //                                    DiscAmtCur = x.DiscAmtCur,
        //                                    TotalAmt = x.TotalAmt,
        //                                    TotalAmtCur = x.TotalAmtCur,
        //                                    TaxBaseAmt = x.TaxBaseAmt,
        //                                    TaxBaseAmtCur = x.TaxBaseAmtCur,
        //                                    VatRate = x.VatRate,
        //                                    VatAmt = x.VatAmt,
        //                                    VatAmtCur = x.VatAmtCur,
        //                                    NetAmt = x.NetAmt,
        //                                    NetAmtCur = x.NetAmtCur,
        //                                    Post = x.Post
        //                                }).ToListAsync();

        //    foreach (var taxinvhd in taxinvs)
        //    {
        //        var invno = taxinvhd.DocNo;
        //        taxinvhd.RunNumber = ++runno;                
        //        taxinvhd.DocNo = GenDocNo(query, pattern, (int)taxinvhd.RunNumber);
        //        taxinvhd.DocStatus = "Active";
        //        taxinvhd.DocPattern = pattern;
        //        taxinvhd.CreatedBy = query.User;
        //        taxinvhd.CreatedDate = DateTime.Now;

        //        var items = this.context.SalCreditsaleDts.Where(x => x.CompCode == taxinvhd.CompCode
        //                                               && x.BrnCode == taxinvhd.BrnCode
        //                                               && x.LocCode == taxinvhd.LocCode
        //                                               && x.DocType == taxinvhd.DocType
        //                                               && x.DocNo == invno)
        //                                                .Select(x => new SalTaxinvoiceDt
        //                                                {
        //                                                    CompCode = x.CompCode,
        //                                                    BrnCode = x.BrnCode,
        //                                                    LocCode = x.LocCode,
        //                                                    DocNo = taxinvhd.DocNo,
        //                                                    SeqNo = x.SeqNo,
        //                                                    PdId = x.PdId,
        //                                                    PdName = x.PdName,
        //                                                    IsFree = x.IsFree,
        //                                                    UnitId = x.UnitId,
        //                                                    UnitBarcode = x.UnitBarcode,
        //                                                    UnitName = x.UnitName,
        //                                                    ItemQty = x.ItemQty,
        //                                                    StockQty = x.StockQty,
        //                                                    UnitPrice = x.UnitPrice,
        //                                                    UnitPriceCur = x.UnitPriceCur,
        //                                                    SumItemAmt = x.SumItemAmt,
        //                                                    SumItemAmtCur = x.SumItemAmtCur,
        //                                                    DiscAmt = x.DiscAmt,
        //                                                    DiscAmtCur = x.DiscAmtCur,
        //                                                    DiscHdAmt = x.DiscHdAmt,
        //                                                    DiscHdAmtCur = x.DiscHdAmtCur,
        //                                                    SubAmt = x.SubAmt,
        //                                                    SubAmtCur = x.SubAmtCur,
        //                                                    VatType = x.VatType,
        //                                                    VatRate = x.VatRate,
        //                                                    VatAmt = x.VatAmt,
        //                                                    VatAmtCur = x.VatAmtCur,
        //                                                    TaxBaseAmt = x.TaxBaseAmt,
        //                                                    TaxBaseAmtCur = x.TaxBaseAmtCur,
        //                                                    TotalAmt = x.TotalAmt,
        //                                                    TotalAmtCur = x.TotalAmtCur
        //                                                }).ToList();

        //        this.context.SalTaxinvoiceDts.AddRange(items);
        //        this.context.SalTaxinvoiceHds.Add(taxinvhd);

        //        var balance = new FinBalance
        //        {
        //            CompCode = taxinvhd.CompCode,
        //            BrnCode = taxinvhd.BrnCode,
        //            LocCode = taxinvhd.LocCode,
        //            DocType = "TaxInvoice",
        //            DocNo = taxinvhd.DocNo,                    
        //            DocDate = taxinvhd.DocDate,
        //            CustCode = taxinvhd.CustCode,
        //            Currency = taxinvhd.Currency,
        //            NetAmt = taxinvhd.NetAmt,
        //            NetAmtCur = taxinvhd.NetAmtCur,
        //            BalanceAmt = taxinvhd.NetAmt,
        //            BalanceAmtCur = taxinvhd.NetAmtCur,
        //            CreatedBy = query.User,
        //            CreatedDate = DateTime.Now
        //        };
        //        this.context.FinBalances.Add(balance);

        //        //update salehd
        //        var salehds = await this.context.SalCreditsaleHds.Where(x => (x.TxNo == null || x.TxNo == "")
        //                                        && x.Post == taxinvhd.Post
        //                                        && x.CompCode == taxinvhd.CompCode
        //                                        && x.BrnCode == taxinvhd.BrnCode
        //                                        && x.LocCode == taxinvhd.LocCode
        //                                        && x.DocType == taxinvhd.DocType
        //                                        && x.DocNo == invno
        //                                        ).ToListAsync();
        //        salehds.ForEach(x => {
        //            x.DocStatus = "Reference";
        //            x.TxNo = taxinvhd.DocNo;
        //            x.UpdatedBy = query.User;
        //            x.UpdatedDate = DateTime.Now;
        //        });
        //        this.context.SalCreditsaleHds.UpdateRange(salehds);



        //    }//end for


        //}



        protected int GetRunNumber(string comp_code, string brn_code, string pattern)
        {
            int runNumber = 0;
            SalTaxinvoiceHd resp = new SalTaxinvoiceHd();
            resp = this.context.SalTaxinvoiceHds.Where(x => x.CompCode == comp_code && x.BrnCode == brn_code && x.DocPattern == pattern).OrderByDescending(x => x.RunNumber).FirstOrDefault();
            if (resp != null)
            {
                runNumber = (resp.RunNumber ?? 0);
            }
            return runNumber;
        }


        protected string GenDocNo(TransferDataResource query, string pattern, int runNumber)
        {
            string docno = "";

            var date = query.WDate.Value;
            var Brn = query.BrnCode;

            var patterns = (from hd in this.context.MasDocPatterns
                            join dt in this.context.MasDocPatternDts on hd.DocId equals dt.DocId
                            where hd.DocType == "TaxInvoice"
                            select dt).ToList();

            docno = pattern;
            docno = docno.Replace("Pre", patterns.FirstOrDefault(x => x.DocCode == "[Pre]").DocValue);
            docno = docno.Replace("#", "") + runNumber.ToString("D" + patterns.FirstOrDefault(x => x.DocCode == "[#]").DocValue);
            return docno;
        }


        public async Task AddCreditSaleAsync(IEnumerable<SalCreditsaleHd> salCreditsaleHds)
        {
            await context.SalCreditsaleHds.AddRangeAsync(salCreditsaleHds);
        }

        public async Task AddCashsaleAsync(IEnumerable<SalCashsaleHd> salCashsaleHds)
        {

            await context.SalCashsaleHds.AddRangeAsync(salCashsaleHds);
        }

        public async Task<List<SalCashsaleHd>> ListCashSaleAsync(TransferDataResource query)
        {
            var casleSales = new List<SalCashsaleHd>();

            var queryable = context.SalCashsaleHds
                         .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post == "P")
                         .AsNoTracking();

            var salCashsaleHds = await queryable.ToListAsync();

            List<SalCashsaleHd> heads = await queryable.ToListAsync();
            heads.ForEach(x => x.SalCashsaleDt = context.SalCashsaleDts.Where(dt => dt.CompCode == x.CompCode
                                                                                           && dt.BrnCode == x.BrnCode
                                                                                           && dt.DocNo == x.DocNo)
                                                                                        .OrderBy(y => y.SeqNo).ToList());

            return heads;
        }

        public async Task<List<SalCreditsaleHd>> ListCreditSaleAsync(TransferDataResource query)
        {
            var creditSales = new List<SalCreditsaleHd>();

            var queryable = context.SalCreditsaleHds
                         .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode
                                    && x.DocType == query.DocType
                                    && x.DocDate == query.WDate && x.Post == query.Post)
                         .AsNoTracking();

            var salCreditsaleHds = await queryable.ToListAsync();


            foreach (var salCreditsaleHd in salCreditsaleHds)
            {
                var Creditsale = new SalCreditsaleHd();
                var salCreditsaleDts = context.SalCreditsaleDts.AsNoTracking().Where(x => x.CompCode == salCreditsaleHd.CompCode && x.BrnCode == salCreditsaleHd.BrnCode && x.DocNo == salCreditsaleHd.DocNo).OrderBy(y => y.SeqNo).ToList();

                if (salCreditsaleDts != null)
                {
                    Creditsale.CompCode = salCreditsaleHd.CompCode;
                    Creditsale.BrnCode = salCreditsaleHd.BrnCode;
                    Creditsale.LocCode = salCreditsaleHd.LocCode;
                    Creditsale.DocNo = salCreditsaleHd.DocNo;
                    Creditsale.DocStatus = salCreditsaleHd.DocStatus;
                    Creditsale.DocDate = salCreditsaleHd.DocDate;
                    Creditsale.RefNo = salCreditsaleHd.RefNo;
                    Creditsale.ItemCount = salCreditsaleHd.ItemCount;
                    Creditsale.Currency = salCreditsaleHd.Currency;
                    Creditsale.CurRate = salCreditsaleHd.CurRate;
                    Creditsale.SubAmt = salCreditsaleHd.SubAmt;
                    Creditsale.SubAmtCur = salCreditsaleHd.SubAmtCur;
                    Creditsale.DiscRate = salCreditsaleHd.DiscRate;
                    Creditsale.DiscAmt = salCreditsaleHd.DiscAmt;
                    Creditsale.DiscAmtCur = salCreditsaleHd.DiscAmtCur;
                    Creditsale.TotalAmt = salCreditsaleHd.TotalAmt;
                    Creditsale.TotalAmtCur = salCreditsaleHd.TotalAmtCur;
                    Creditsale.TaxBaseAmt = salCreditsaleHd.TaxBaseAmt;
                    Creditsale.TaxBaseAmtCur = salCreditsaleHd.TaxBaseAmtCur;
                    Creditsale.VatRate = salCreditsaleHd.VatRate;
                    Creditsale.VatAmt = salCreditsaleHd.VatAmt;
                    Creditsale.VatAmtCur = salCreditsaleHd.VatAmtCur;
                    Creditsale.NetAmt = salCreditsaleHd.NetAmt;
                    Creditsale.NetAmtCur = salCreditsaleHd.NetAmtCur;

                    Creditsale.Post = salCreditsaleHd.Post;
                    Creditsale.RunNumber = salCreditsaleHd.RunNumber;
                    Creditsale.Guid = salCreditsaleHd.Guid;
                    Creditsale.CreatedDate = salCreditsaleHd.CreatedDate;
                    Creditsale.CreatedBy = salCreditsaleHd.CreatedBy;
                    Creditsale.UpdatedBy = salCreditsaleHd.UpdatedBy;
                    Creditsale.UpdatedDate = salCreditsaleHd.UpdatedDate;
                    Creditsale.SalCreditsaleDt = salCreditsaleDts;

                    creditSales.Add(Creditsale);
                }
            }

            return creditSales;
        }


    }
}
