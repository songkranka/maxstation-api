using AutoMapper;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PostDay.API.Domain.Models;
using PostDay.API.Domain.Models.PostDay;
using PostDay.API.Domain.Models.Request;
using PostDay.API.Domain.Models.Response;
using PostDay.API.Domain.Models.RestAPI;
using PostDay.API.Domain.Models.Soap;
using PostDay.API.Domain.Repositories;
using PostDay.API.Domain.Services.Communication.PostDay;
using PostDay.API.Helpers;
using PostDay.API.Resources.PostDay;
using PostDay.API.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace PostDay.API.Repositories
{
    public class PostDayRepository : SqlDataAccessHelper, IPostDayRepository
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PostDayRepository(
            PTMaxstationContext context,
            IMapper mapper,
            IUnitOfWork unitOfWork) : base(context)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        public async Task UpdateCreditSaleAsync(PostDayResource query)
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateCreditSaleAsync: {strMessage}");
            }
        }

        public async Task CreateTaxInvoiceAsync(PostDayResource query)
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

            this.context.SaveChanges();
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
                    var customer = this.context.MasCustomers.Find(cust.CustCode);

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
                    taxinvoicehd.CustName = cust.CustName;
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
                                                           && docnos.Contains(d.DocNo)).Select(x => x.LicensePlate).Distinct().ToList();
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
                        taxinvoicehd.CustName = salehd.CustName;
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
                                                                    ItemQty = x.ItemQty ?? decimal.Zero,
                                                                    StockQty = x.StockQty ?? decimal.Zero,
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
            var taxinvs = await (from cre in this.context.SalCreditsaleHds
                             join cus in this.context.MasCustomers on cre.CustCode equals cus.CustCode
                             where cre.CompCode == query.CompCode
                            && cre.BrnCode == query.BrnCode
                            && cre.Post == "P"
                            && cre.DocType == "Invoice"
                            && (cre.TxNo == "" || cre.TxNo == null)
                            select new { cre,cus}).Select(x => new SalTaxinvoiceHd
                                        {
                                            CompCode = x.cre.CompCode,
                                            BrnCode = x.cre.BrnCode,
                                            LocCode = x.cre.LocCode,
                                            DocType = x.cre.DocType,
                                            DocNo = x.cre.DocNo,
                                            DocDate = x.cre.DocDate,
                                            CustCode = x.cre.CustCode,
                                            CustName = x.cre.CustName,
                                            CustAddr1 = x.cre.CustAddr1,
                                            CustAddr2 = x.cre.CustAddr2,
                                            CitizenId = x.cus.CitizenId,
                                            Currency = x.cre.Currency,
                                            CurRate = x.cre.CurRate,
                                            ItemCount = x.cre.ItemCount,
                                            SubAmt = x.cre.SubAmt,
                                            SubAmtCur = x.cre.SubAmtCur,
                                            DiscRate = x.cre.DiscRate,
                                            DiscAmt = x.cre.DiscAmt,
                                            DiscAmtCur = x.cre.DiscAmtCur,
                                            TotalAmt = x.cre.TotalAmt,
                                            TotalAmtCur = x.cre.TotalAmtCur,
                                            TaxBaseAmt = Math.Round((decimal)x.cre.TaxBaseAmt, 2),
                                            TaxBaseAmtCur = Math.Round((decimal)x.cre.TaxBaseAmtCur, 2),
                                            VatRate = x.cre.VatRate,
                                            VatAmt = Math.Round((decimal)x.cre.VatAmt, 2),
                                            VatAmtCur = Math.Round((decimal)x.cre.VatAmtCur, 2),
                                            NetAmt = Math.Round((decimal)x.cre.NetAmt, 2),
                                            NetAmtCur = Math.Round((decimal)x.cre.NetAmtCur, 2),
                                            Post = x.cre.Post
                                        }).ToListAsync();


            foreach (var taxinvhd in taxinvs)
            {
                var invno = taxinvhd.DocNo;
                taxinvhd.RunNumber = ++runno;
                taxinvhd.DocNo = GenDocNo(query, pattern, (int)taxinvhd.RunNumber);
                taxinvhd.DocStatus = "Active";
                taxinvhd.DocPattern = pattern;

                List<string> pdmeter = new List<string>() { "90575", "90581" };

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
                                                            PdName = x.PdName + (pdmeter.Contains(x.PdId)? $"({x.MeterStart} - {x.MeterFinish})":"" ),
                                                            IsFree = x.IsFree,
                                                            UnitId = x.UnitId,
                                                            UnitBarcode = x.UnitBarcode,
                                                            UnitName = x.UnitName,
                                                            ItemQty = x.ItemQty ?? decimal.Zero,
                                                            StockQty = x.StockQty ?? decimal.Zero,
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

        protected string GenDocNo(PostDayResource query, string pattern, int runNumber)
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

        public async Task<PostDayResponse> GetDocument(GetDocumentRequest req)
        {
            try
            {
                var checkCoperate = "Yes";
                var _sourcenameCredit = "POSCredit";
                var masCompany = await context.MasCompanies.FirstOrDefaultAsync(x => x.CompCode == req.CompCode);
                var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var hdData = await context.DopPostdayHds.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode && x.DocDate == docDate).AsNoTracking().FirstOrDefaultAsync();

                var listDtData = await context.DopPostdayDts.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode && x.DocDate == docDate).AsNoTracking().ToListAsync();
                var listCr = listDtData.Where(x => x.DocType == "CR").ToList();
                var listDr = listDtData.Where(x => x.DocType == "DR").ToList();

                var listFmData = await context.DopPostdaySums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode && x.DocDate == docDate).AsNoTracking().ToListAsync();
                var masCustomers = context.MasCustomers.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode);
                var masBranchConfig = await context.MasBranchConfigs.FirstOrDefaultAsync(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode); 
                var listResultFormula = new List<Formula>();

                if (listFmData.Count > 0)
                {
                    listResultFormula = _mapper.Map<List<DopPostdaySum>, List<Formula>>(listFmData);
                    for (int i = 0; i < listResultFormula.Count; i++)
                    {
                        listResultFormula[i].Unit = listFmData[i].UnitName;
                    }
                }
                else
                {
                    var listFm = await (from brn in context.DopFormulaBranches
                                        join sql in context.DopFormulas on brn.FmNo equals sql.FmNo
                                        where brn.CompCode == req.CompCode
                                        && brn.BrnCode == req.BrnCode
                                        select sql).AsNoTracking().ToListAsync();

                    if(masBranchConfig != null)
                    {
                        if (masBranchConfig.IsLockSlip != "Y" || masBranchConfig.IsPos != "Y")
                        {
                            var posCreditApi = listFm.FirstOrDefault(x => x.SourceName == _sourcenameCredit);
                            listFm.Remove(posCreditApi);
                        }
                    }

                    var listResultFm = new List<Formula>();
                    foreach (var fm in listFm)
                    {
                        var resultFm = new Formula()
                        {
                            FmNo = fm.FmNo,
                            Remark = fm.Remark,
                            Unit = fm.UnitName
                        };

                        if (fm.SourceType == "SQL")
                        {
                            var sourceQuery = fm.SourceValue
                                .Replace("@COMP_CODE", $"'{req.CompCode}'").Replace("@comp_code", $"'{req.CompCode}'")
                                .Replace("@BRN_CODE", $"'{req.BrnCode}'").Replace("@brn_code", $"'{req.BrnCode}'")
                                .Replace("@LOC_CODE", $"'{req.LocCode}'").Replace("@loc_code", $"'{req.LocCode}'")
                                .Replace("@DOC_DATE", $"'{req.DocDate}'").Replace("@doc_date", $"'{req.DocDate}'");

                            resultFm.SourceAmount = GetValueFromQuery(sourceQuery, context);
                        }
                        else if (fm.SourceType == "API")
                        {
                            if(fm.SourceName == "MaxPosSum")
                            {
                                var maxPosSum = await GetMaxPosSum(fm.SourceValue, fm.SourceKey, docDate, req.BrnCode);
                                resultFm.SourceAmount = Math.Round(Convert.ToDecimal(maxPosSum), 2);
                            }
                            else if(fm.SourceName == "PCCA")
                            {
                                var pccaValue = await GetPCCAService(fm.SourceValue, req.CompCode, req.BrnCode, req.DocDate);
                                resultFm.SourceAmount = pccaValue;
                            }
                            else if(fm.SourceName == _sourcenameCredit)
                            {
                                var posCreditValue = await GetPOSCreditAmount(fm.SourceValue, req.BrnCode, docDate);
                                resultFm.SourceAmount = posCreditValue;
                            }
                            
                            else
                            {
                                var sourceValue = await GetValueFromApi(fm.SourceValue, fm.SourceKey, masCompany.CompSname, req.BrnCode, req.DocDate);
                                resultFm.SourceAmount = sourceValue;
                            }
                        }

                        if (fm.DestinationType == "SQL")
                        {
                            var destinationQuery = fm.DestinationValue
                                .Replace("@COMP_CODE", $"'{req.CompCode}'").Replace("@comp_code", $"'{req.CompCode}'")
                                .Replace("@BRN_CODE", $"'{req.BrnCode}'").Replace("@brn_code", $"'{req.BrnCode}'")
                                .Replace("@LOC_CODE", $"'{req.LocCode}'").Replace("@loc_code", $"'{req.LocCode}'")
                                .Replace("@DOC_DATE", $"'{req.DocDate}'").Replace("@doc_date", $"'{req.DocDate}'");

                            resultFm.DestinationAmount = GetValueFromQuery(destinationQuery, context);
                        }

                        listResultFm.Add(resultFm);
                    }

                    listResultFormula = listResultFm;
                }

                var listCashAmt = new List<decimal>();
                var listDiffAmt = new List<decimal>();

                var meterData = await context.DopPeriodCashSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate).AsNoTracking().ToListAsync();
                meterData.ForEach(x =>
                {
                    listCashAmt.Add(x.CashAmt.Value);
                    listDiffAmt.Add(x.DiffAmt.Value);
                });

                var payType = new List<string>()
                {
                    "2",
                    "เช็ค"
                };

                var receiveData = await (from hd in context.FinReceiveHds
                                         join dt in context.FinReceiveDts on hd.DocNo equals dt.DocNo
                                         where hd.CompCode == req.CompCode && hd.BrnCode == req.BrnCode && hd.LocCode == req.LocCode && hd.DocDate == docDate && payType.Contains(hd.PayType)
                                         select dt.ItemAmt.Value).ToListAsync();

                var sumData = new SumInDay()
                {
                    SumCashAmt = listCashAmt.Sum(),
                    SumDiffAmt = listDiffAmt.Sum(),
                    SumCashDepositAmt = listCashAmt.Sum() + listDiffAmt.Sum(),
                    SumChequeAmt = receiveData.Sum()
                };

                var listCheckBeforeSave = new List<CheckBeforeSave>();
                var dopValidates = await context.DopValidates.Where(x => x.ValidStatus == "Active").ToListAsync();
                //var validateData = dopValidates.Where(x => x.SourceType == "SQL");
                //var validateCoperate = dopValidates.FirstOrDefault(x => x.SourceType == "SOAP");

                foreach (var item in dopValidates)
                {

                    if (item.SourceType == "SOAP")
                    {

                        if (item.SourceKey == "Corporate")
                        {

                            var customerSoaps = await GetValueFronSoap(req.BrnCode, docDate, item.SourceValue);
                            var creditsales = context.SalCreditsaleHds.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode &&
                                                                            x.DocDate == docDate && x.DocStatus != "Cancel" && x.DocType == "CreditSale")
                                            .GroupBy(c => c.CustCode)
                                            .Select(cd => new SalCreditsaleHd
                                            {
                                                CustCode = cd.Key,
                                                SubAmt = cd.Sum(x => x.SubAmt)
                                            })
                                           .ToList();
                            
                            if (customerSoaps != null && customerSoaps.Count() > 0)
                            {
                                foreach (var customersoap in customerSoaps)
                                {
                                    var creditsale = creditsales.FirstOrDefault(x => x.CustCode.Trim() == customersoap.legcustcode.Trim());

                                    if (creditsale == null)
                                    {
                                        checkCoperate = "No";
                                        break;
                                    }
                                    else
                                    {
                                        if (customersoap.amt != creditsale.SubAmt)
                                        {
                                            checkCoperate = "No";
                                            break;
                                        }
                                    }
                                }
                            }

                            listCheckBeforeSave.Add(new CheckBeforeSave() { 
                                Label = item.Remark, 
                                PassValue = checkCoperate ,
                                ValidNo = item.ValidNo ,
                                HaveValidSql = false,
                            });
                        }


                    }
                    else if (item.SourceType == "API" && item.SourceKey == "GetPeriodCount")
                    {
                        var isPos = GetIsPosFromMasBranchConfig(req.CompCode, req.BrnCode);

                        if (isPos)
                        {
                            var passValue = await ComparePeriod(req.CompCode, req.BrnCode, docDate, item.SourceValue);
                            listCheckBeforeSave.Add(new CheckBeforeSave()
                            {
                                Label = item.Remark,
                                PassValue = passValue,
                                ValidNo = item.ValidNo,
                                HaveValidSql = !string.IsNullOrWhiteSpace(item.ValidSql),
                            });
                        }
                    }
                    else // sql
                    {
                        var query = item.SourceValue
                        .Replace("@COMP_CODE", $"'{req.CompCode}'").Replace("@comp_code", $"'{req.CompCode}'")
                        .Replace("@BRN_CODE", $"'{req.BrnCode}'").Replace("@brn_code", $"'{req.BrnCode}'")
                        .Replace("@LOC_CODE", $"'{req.LocCode}'").Replace("@loc_code", $"'{req.LocCode}'")
                        .Replace("@DOC_DATE", $"'{req.DocDate}'").Replace("@doc_date", $"'{req.DocDate}'");
                        listCheckBeforeSave.Add(new CheckBeforeSave() { 
                            Label = item.Remark, 
                            PassValue = GetValueFromQueryValidate(query, context) ,
                            ValidNo = item.ValidNo,
                            HaveValidSql = !string.IsNullOrWhiteSpace(item.ValidSql),
                        });

                    }

                }



                //check all meter data is active
                //var listMeterData = await context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate).AsNoTracking().ToListAsync();
                //var checkAllTransIsActive = listMeterData.Where(x => x.PeriodStatus != "Active").ToList();
                //if (listMeterData.Count == 0 || checkAllTransIsActive.Count > 0)
                //{
                //    listCheckBeforeSave.Add(new CheckBeforeSave() { Label = "บันทึกมิเตอร์/วัดถัง/รับจ่าย", PassValue = "No" });
                //}
                //else 
                //{
                //    listCheckBeforeSave.Add(new CheckBeforeSave() { Label = "บันทึกมิเตอร์/วัดถัง/รับจ่าย", PassValue = "Yes" });
                //}

                var result = new GetDocumentResponse()
                {
                    DopPostdayHd = hdData,
                    CrItems = listCr,
                    DrItems = listDr,
                    FormulaItems = listResultFormula,
                    SumData = sumData,
                    CheckBeforeSaveItems = listCheckBeforeSave
                };

                return new PostDayResponse(result);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new PostDayResponse($"An error occurred when get document postday : {ex.Message}");
            }
        }

        private async Task<List<CustomerAmount>> GetValueFronSoap(string brnCode, DateTime docDate, string sourceValue)
        {
            var response = new List<CustomerAmount>();
            try
            {

                var tomorrow = docDate.Date.AddDays(1);
                var startDate = new DateTime(docDate.Year, docDate.Month, docDate.Day, 05, 00, 00, 000).ToString("yyyy-MM-dd HH:mm:ss.fff");
                var endDate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 04, 59, 59, 999).ToString("yyyy-MM-dd HH:mm:ss.fff");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sourceValue); //New Link 16/10/2020
                request.Headers.Add(@"SOAPAction:http://tempuri.org/Web_station_GetAmt");
                request.ContentType = "text/xml;charset=\"utf-8\"";
                request.Accept = "text/xml";
                request.Method = "POST";
                string jsonText = "{\"W_brn\":{\"brn\":\"" + brnCode + "\"},\"S_date\":{\"strdate\":\"" + startDate + "\",\"enddate\":\"" + endDate + "\"  }}";

                ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                XmlDocument SOAPReqBody = new XmlDocument();
                SOAPReqBody.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
            <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-   instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">  
             <soap:Body>  
                <Web_station_GetAmt  xmlns=""http://tempuri.org/"">  
                   <strJson>" + jsonText + @"</strJson>
                </Web_station_GetAmt>  
              </soap:Body>  
            </soap:Envelope>");

                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    SOAPReqBody.Save(stream);
                }

                using (WebResponse Serviceres = await request.GetResponseAsync())
                {
                    using StreamReader rd = new StreamReader(Serviceres.GetResponseStream());
                    string serviceResult = rd.ReadToEnd();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(serviceResult);

                    XmlNodeList elemList = doc.GetElementsByTagName("Web_station_GetAmtResult");
                    string jsonString = elemList[0].InnerXml;
                    JObject jObjectData = JObject.Parse(jsonString);
                    string tagData = jObjectData["Data"].ToString();
                    JObject jObjectDetail = JObject.Parse(tagData);
                    string tagDetail = jObjectDetail["Detail"].ToString();
                    response = JsonConvert.DeserializeObject<List<CustomerAmount>>(tagDetail);
                }
                return response;
            }
            catch(Exception ex) {
                throw new Exception("ไม่สามารถเชื่อมต่อกับระบบ Coperate ได้");
            }
           
        }

        private async Task<decimal> GetPCCAService(string url, string compCode, string brnCode, string docDate)
        {
            var request = new PCCARequest()
            {
                CODCOMP_HEAD = compCode,
                CENTER_CODE = brnCode,
                H_TS = docDate
            };

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(url);
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                response.Content.ReadAsStringAsync().Wait();
                var jsonString = response.Content.ReadAsStringAsync().Result;

                var responseData = JsonConvert.DeserializeObject<PCCA>(jsonString);

                return responseData.AMOUNT;
            }
            else
            {
                throw new Exception($"Call api failed with url : {url}");
            }
        }

        private async Task<decimal> GetValueFromApi(string url, string reqtKey, string compSname, string brnCode, string docDate)
        {

            var req = new RequestFormula()
            {
                ReqtKey = reqtKey,
                CompanyCode = compSname,
                ShopCode = brnCode,
                SystemDate = docDate
            };


            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(url);
            var content = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                response.Content.ReadAsStringAsync().Wait();
                var jsonString = response.Content.ReadAsStringAsync().Result;

                var responseData = JsonConvert.DeserializeObject<ResponseFormula>(jsonString);

                return responseData.Detail.Discount;
            }
            else
            {
                throw new Exception($"Call api failed with url : {url}");
            }
        }

        private async Task<decimal> GetPOSCreditAmount(string url, string brnCode, DateTime docDate)
        {
            //url = "https://prod-maxstation-dailyoperation-web1-asv.azurewebsites.net/api/Pos/GetCreditSummaryByBranch";
            //docDate = DateTime.Now;

            var request = new POSCreditRequest()
            {
                BrnCode = brnCode,
                FromDate = docDate
            };

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            client.BaseAddress = new Uri(url);
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                response.Content.ReadAsStringAsync().Wait();
                var jsonString = response.Content.ReadAsStringAsync().Result;
                var jsonDeserializeObject = JsonConvert.DeserializeObject(jsonString).ToString();
                var jsonObject = JObject.Parse(jsonDeserializeObject);
                var responseData = jsonObject.ToObject<PostCreditResponse>();

                return responseData.Data.Amount;
            }
            else
            {
                throw new Exception($"Call api failed with url : {url}");
            }
        }


        public async Task<string> GetMaxPosSum(string url, string token, DateTime docDate, string shopId)
        {
            try
            {
                var tomorrow = docDate.Date.AddDays(1);
                var startTime = new DateTime(docDate.Year, docDate.Month, docDate.Day, 05, 00, 00, 000);
                var endTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 04, 59, 59, 999);

                var request = new MaxPosSumRequest()
                {
                    token = token,
                    starttime = startTime,
                    endtime = endTime,
                    shopid = shopId
                };

                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(url);
                //client.BaseAddress = new Uri("https://api.uat.maxcard.tech/api/inquiry/CS/api/maxpossum");
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                //var response = await client.PostAsync("https://api.uat.maxcard.tech/api/inquiry/CS/api/maxpossum", content);

                if (response.IsSuccessStatusCode)
                {
                    response.Content.ReadAsStringAsync().Wait();
                    var jsonString = response.Content.ReadAsStringAsync().Result;

                    var responseData = JsonConvert.DeserializeObject<MaxPosSumResponse>(jsonString);

                    return responseData.summary;
                }
                else
                {
                    throw new Exception($"Call api failed with url : {url}");
                }
            }
            catch
            {
                return "0";
            }

        }

        private decimal GetValueFromQuery(string query, DbContext dbContext)
        {
            var cnn = dbContext.Database.GetDbConnection();
            var cmm = cnn.CreateCommand();
            cmm.Connection = cnn;
            cmm.CommandText = query;
            cnn.Open();
            var reader = cmm.ExecuteScalar();
            var result = reader != DBNull.Value ? Convert.ToDecimal(reader) : 0;
            cnn.Close();

            return result;
        }

        private string GetValueFromQueryValidate(string query, DbContext dbContext)
        {
            var cnn = dbContext.Database.GetDbConnection();
            var cmm = cnn.CreateCommand();
            cmm.Connection = cnn;
            cmm.CommandText = query;
            cnn.Open();
            var reader = cmm.ExecuteScalar();
            var result = reader != DBNull.Value ? Convert.ToString(reader) : "";
            cnn.Close();

            return result;
        }

        private bool GetIsPosFromMasBranchConfig(string compCode, string brnCode)
        {
            try
            {
                var masBranchConfig = context.MasBranchConfigs.FirstOrDefault(x => x.CompCode == compCode && x.BrnCode == brnCode);

                if (masBranchConfig != null && masBranchConfig.IsPos == "Y" && masBranchConfig.IsLockMeter == "Y")
                {
                    return true;   // ต้องตรวจสอบ
                }
                else
                {
                    return false; // ไม่ต้องตรวจสอบ ข้ามข้อนี้ไปเลย
                } 
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private async Task<string> ComparePeriod(string compCode, string brnCode, DateTime docDate, string sourceValue)
        {
            try
            {
                var result = string.Empty;
                var maxPeriod = context.DopPeriods
                                    .Where(x => x.CompCode == compCode && x.BrnCode == brnCode && x.DocDate == docDate)
                                    .Select(d => d.PeriodNo).Distinct()
                                    .Count();
                

                var request = new POSPeriodCountRequest()
                {
                    BrnCode = brnCode,
                    FromDate = docDate
                };

                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(sourceValue);
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(sourceValue, content);

                if (response.IsSuccessStatusCode)
                {
                    response.Content.ReadAsStringAsync().Wait();
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    var jsonDeserializeObject = JsonConvert.DeserializeObject(jsonString).ToString();
                    var jsonObject = JObject.Parse(jsonDeserializeObject);
                    var responseData = jsonObject.ToObject<POSPeriodCountResponse>();
                    var posPeriod =  responseData.Data.CountPeriod;

                    if (maxPeriod == posPeriod) 
                    {
                        result = "Yes";
                    }
                    else
                    {
                        result = "No";
                    }
                }
                else
                {
                    throw new Exception($"Call api failed with url : {sourceValue}");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred when compare period : {ex.Message}");
            }
        }

        public async Task<SaveDocumentResponse> SaveDocument(SaveDocumentRequest req)
        {
            try
            {
                var docDate = DateTime.ParseExact(req.DopPostdayHd.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var oldHdData = await context.DopPostdayHds.Where(x => x.CompCode == req.DopPostdayHd.CompCode && x.BrnCode == req.DopPostdayHd.BrnCode && x.LocCode == req.DopPostdayHd.LocCode && x.DocDate == docDate).AsNoTracking().FirstOrDefaultAsync();
                var listOldDtData = await context.DopPostdayDts.Where(x => x.CompCode == req.DopPostdayHd.CompCode && x.BrnCode == req.DopPostdayHd.BrnCode && x.LocCode == req.DopPostdayHd.LocCode && x.DocDate == docDate).AsNoTracking().ToListAsync();
                var listOldPostDaySumData = await context.DopPostdaySums.Where(x => x.CompCode == req.DopPostdayHd.CompCode && x.BrnCode == req.DopPostdayHd.BrnCode && x.LocCode == req.DopPostdayHd.LocCode && x.DocDate == docDate).AsNoTracking().ToListAsync();
                var listOldPostDayValidate = await context.DopPostdayValidates.Where(x => x.CompCode == req.DopPostdayHd.CompCode && x.BrnCode == req.DopPostdayHd.BrnCode && x.LocCode == req.DopPostdayHd.LocCode && x.DocDate == docDate).AsNoTracking().ToListAsync();
                var newHdData = _mapper.Map<CtDopPostdayHd, DopPostdayHd>(req.DopPostdayHd);
                newHdData.DocDate = docDate;
                newHdData.CreatedDate = DateTime.Now;

                var joinListDt = req.CrItems.Concat(req.DrItems).ToList();
                joinListDt.ForEach(x =>
                {
                    x.DocDate = docDate;
                });

                var listInsertDopPostdaySum = _mapper.Map<List<Formula>, List<DopPostdaySum>>(req.FormulaItems);
                var seq = 1;
                listInsertDopPostdaySum.ForEach(x =>
                {
                    x.CompCode = req.DopPostdayHd.CompCode;
                    x.BrnCode = req.DopPostdayHd.BrnCode;
                    x.LocCode = req.DopPostdayHd.LocCode;
                    x.DocDate = docDate;
                    x.SeqNo = seq++;
                    x.UnitName = req.FormulaItems.FirstOrDefault(y => x.FmNo == y.FmNo)?.Unit ?? string.Empty;
                });


                if (oldHdData != null)
                {
                    context.DopPostdayHds.Remove(oldHdData);
                }
                context.DopPostdayDts.RemoveRange(listOldDtData);
                context.DopPostdaySums.RemoveRange(listOldPostDaySumData);
                context.DopPostdayValidates.RemoveRange(listOldPostDayValidate);

                await context.DopPostdayHds.AddAsync(newHdData);
                await context.DopPostdayDts.AddRangeAsync(joinListDt);
                await context.DopPostdaySums.AddRangeAsync(listInsertDopPostdaySum);
                //context.DopPos
                if (req.CheckBeforeSaveItems?.Any() ?? false)
                {
                    var arrPDV = req.CheckBeforeSaveItems.Select((x, i) => new DopPostdayValidate()
                    {
                        BrnCode = req.DopPostdayHd.BrnCode,
                        CompCode = req.DopPostdayHd.CompCode,
                        LocCode = req.DopPostdayHd.LocCode,
                        DocDate = docDate,
                        SeqNo = i + 1,
                        ValidRemark = x.Label,
                        ValidResult = x.PassValue
                    }).ToArray();
                    await context.DopPostdayValidates.AddRangeAsync(arrPDV);
                }
                var updateClosedayRequest = new PostDayResource()
                {
                    CompCode = req.DopPostdayHd.CompCode,
                    BrnCode = req.DopPostdayHd.BrnCode,
                    WDate = docDate,
                    User = req.DopPostdayHd.User
                };
                this.context.SaveChanges();

                await this.UpdateCloseDayAsync(updateClosedayRequest);



                string strSpAddStock = @"exec sp_add_stock_daily @p0 ,@p1,@p2,@p3 , @p4";

                //var _curCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                //var beforeDate = DateTime.ParseExact(req.DopPostdayHd.DocDate, "yyyy-MM-dd", _curCulture);
                //object[] paramsBefore = new[] {
                //    req.DopPostdayHd.CompCode,
                //    req.DopPostdayHd.BrnCode,
                //    req.DopPostdayHd.LocCode,
                //    req.DopPostdayHd.User,
                //   beforeDate.AddDays(-1).ToString("yyyy-MM-dd",_curCulture)
                //};
                //context.Database.ExecuteSqlRaw(strSpAddStock, paramsBefore);

                object[] arrAddStockParam = new[] {
                    req.DopPostdayHd.CompCode,
                    req.DopPostdayHd.BrnCode,
                    req.DopPostdayHd.LocCode,
                    req.DopPostdayHd.User,
                    req.DopPostdayHd.DocDate
                };                
                context.Database.ExecuteSqlRaw(strSpAddStock, arrAddStockParam);


                var docYear = docDate.Year;
                var docMonth = docDate.Month;
                var docDay = docDate.Day;
                if (docDay == DateTime.DaysInMonth(docYear, docMonth))
                {
                    string strSpAddStockMonthly = @"exec sp_add_stock_monthly @p0,@p1,@p2,@p3,@p4,@p5";
                    object[] arrAddStockMonthlyParam = new object[] {
                        req.DopPostdayHd.CompCode,
                        req.DopPostdayHd.BrnCode,
                        req.DopPostdayHd.LocCode,
                        req.DopPostdayHd.User,
                        docYear ,
                        docMonth,
                    };
                    context.Database.ExecuteSqlRaw(strSpAddStockMonthly, arrAddStockMonthlyParam);
                }


                await _unitOfWork.CompleteAsync();

                return new SaveDocumentResponse()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Status = "Success",
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred when save document postday : {ex.Message}");
            }
        }

        public async Task UpdateCloseDayAsync(PostDayResource query)
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
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
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
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
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
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
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
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
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
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
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
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
                ).ToListAsync();

            invReturnSup.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(invReturnSup);
            #endregion

            #region Audit
            var invAudit = await context.InvAuditHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
                ).ToListAsync();

            invAudit.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(invAudit);
            #endregion

            #region Adjust
            var invAdjust = await context.InvAdjustHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
                ).ToListAsync();

            invAdjust.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });

            context.UpdateRange(invAdjust);
            #endregion

            #region ReturnOil
            var invReturnOil = await context.InvReturnOilHds
                .Where(
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
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
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
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
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
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
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
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
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
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
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
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
                    x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.Post != "P"
                ).ToListAsync();

            finReceive.ForEach(x => {
                x.UpdatedBy = query.User;
                x.UpdatedDate = DateTime.Now;
                x.Post = "P";
            });
            context.UpdateRange(finReceive);
            #endregion

            #region BranchConfig

            var branchConfig = await context.MasBranchConfigs.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.IsPos == "Y").FirstOrDefaultAsync();
            if(branchConfig != null)
            {

                var maxseq = this.context.SysBranchConfigs.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode)
                        .OrderBy(x=>x.DocDate).ThenBy(x=>x.SeqNo).LastOrDefault();
                if(maxseq != null)
                {
                    var ismeter = this.context.SysBranchConfigs.FirstOrDefault(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == maxseq.DocDate && x.SeqNo == maxseq.SeqNo && x.ConfigId == "IS_LOCK_METER");
                    if (ismeter != null)
                    {
                        branchConfig.IsLockMeter = (ismeter.EndDate <= query.WDate) ? "Y" : branchConfig.IsLockMeter;
                    }
                    else
                    {
                        branchConfig.IsLockMeter = "Y";
                    }

                    var isslip = this.context.SysBranchConfigs.FirstOrDefault(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == maxseq.DocDate && x.SeqNo == maxseq.SeqNo && x.ConfigId == "IS_LOCK_SLIP");                    
                    if (isslip != null)
                    {
                        branchConfig.IsLockSlip = (isslip.EndDate <= query.WDate) ? "Y" : branchConfig.IsLockSlip;
                    }
                    else
                    {
                        branchConfig.IsLockSlip = "Y";

                    }
                }
                else
                {
                    branchConfig.IsLockMeter = "Y";  //initial
                    branchConfig.IsLockSlip = "Y";  //initial
                }               
                branchConfig.UpdatedBy = query.User;
                branchConfig.UpdatedDate = DateTime.Now;
                context.Update(branchConfig);
            }            
            #endregion

            #region mas_control
            var icControl = await context.MasControls.Where(x => x.CompCode == query.CompCode
                                                                   && x.BrnCode == query.BrnCode
                                                                   && x.CtrlCode == "WDATE").ToListAsync();

            if (icControl == null || icControl.Count() == 0)
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
            ////log.LogNo = (this.context.DopPostdayLogs.Max(e => (int?)e.LogNo) ?? 0) + 1;
            //log.CompCode = query.CompCode;
            //log.BrnCode = query.BrnCode;
            //log.LocCode = "";
            //log.DocNo = "";
            //log.JsonData = JsonConvert.SerializeObject(query);
            //log.CreatedBy = query.User;
            //AddLog(log);
            #endregion

            #region Call warpad when oil less than capacity min

            var lastPeriod = await context.DopPeriodTanks.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate).OrderBy(x => x.PeriodNo).LastOrDefaultAsync();
            if (lastPeriod != null)
            {
                var listTankData = await context.DopPeriodTanks.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate && x.PeriodNo == lastPeriod.PeriodNo).AsNoTracking().ToListAsync();
                var listMasTankData = await context.MasBranchTanks.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode).AsNoTracking().ToListAsync();


                var configApi = context.SysConfigApis.Where(x => x.SystemId == "Warpad" && x.ApiId == "M005").FirstOrDefault();

                if (configApi != null)
                {
                    var devCodeFrom = context.MasOrganizes.Where(x => x.OrgComp == query.CompCode && x.OrgCode == query.BrnCode).Select(x => x.OrgCodedev).FirstOrDefault();

                    var toppic = configApi.Topic.Replace("{doc_date}", query.WDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));

                    var requestWarpad = new RequestWarpadModel()
                    {
                        TOPIC = toppic,
                        CREATE_DATE = query.WDate.Value.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                        CREATE_TIME = query.WDate.Value.ToString("HH:mm", CultureInfo.InvariantCulture),
                        BRANCH_FROM = devCodeFrom ?? "",
                        LINK = "#",
                    };

                    var listItemRequestWarpad = new List<RequestWarpadDataMedel>();

                    foreach (var item in listTankData)
                    {
                        var map = listMasTankData.Where(x => x.TankId == item.TankId).FirstOrDefault();
                        if (map != null)
                        {
                            if (item.RealQty < map.CapacityMin)
                            {
                                listItemRequestWarpad.Add(new RequestWarpadDataMedel() { ITEM = item.PdName });
                            }
                        }
                    }

                    requestWarpad.DATA = listItemRequestWarpad;

                    await SendDataToWarpadAsync(requestWarpad, configApi.ApiUrl);
                }
            }

            #endregion
        }

        public DopPostdayLog AddLog(DopPostdayLog log)
        {
            log.CreatedDate = DateTime.Now;
            this.context.DopPostdayLogs.Add(log);
            return log;
        }

        private async Task< string> getDopValidSql(GetDopValidDataParam param)
        {
            if(param == null)
            {
                return string.Empty;
            }
            string strCon = context.Database.GetConnectionString();            
            string strSql = $"select top 1 VALID_SQL from DOP_VALIDATE(nolock) where VALID_NO={param.ValidNo} and VALID_STATUS='Active'";
            string result = await DefaultService.ExecuteScalar<string>(strCon , strSql);
            if (string.IsNullOrWhiteSpace(result))
            {
                return string.Empty;
            }
            result = Regex.Replace(result, "@comp_code", param.CompCode, RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "@brn_code", param.BrnCode, RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "@loc_code", param.LocCode, RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "@doc_date", param.DocDate, RegexOptions.IgnoreCase);

            return result;
        }

        public async Task< DataTable> GetDopValidData(GetDopValidDataParam param)
        {
            string strSql = await getDopValidSql(param);
            if (string.IsNullOrWhiteSpace(strSql))
            {
                return null;
            }
            string strCon = context.Database.GetConnectionString();
            DataTable result = await DefaultService.GetDataTable(strCon, strSql);
            return result;
        }
    }
}
