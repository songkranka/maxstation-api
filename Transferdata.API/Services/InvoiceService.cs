using MaxStation.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;
using Transferdata.API.Resources;

namespace Transferdata.API.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private PTMaxstationContext Context;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IUnitOfWork unitOfWork,
            PTMaxstationContext context,
            IServiceScopeFactory serviceScopeFactory)
        {
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
            Context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }




        protected int GetRunNumber(string comp_code, string brn_code, string pattern)
        {
            int runNumber = 0;
            var resp = this.Context.SalCreditsaleHds.Where(x => x.CompCode == comp_code && x.BrnCode == brn_code && x.DocPattern == pattern).OrderByDescending(x => x.RunNumber).FirstOrDefault();
            if (resp != null)
            {
                runNumber = (resp.RunNumber ?? 0);
            }
            return runNumber;
        }

        protected string GenDocNo(SalCreditsaleHd query, string pattern, int runNumber)
        {
            string docno = "";

            var date = query.DocDate.Value;
            var Brn = query.BrnCode;

            var patterns = (from hd in this.Context.MasDocPatterns
                            join dt in this.Context.MasDocPatternDts on hd.DocId equals dt.DocId
                            where hd.DocType == "Invoice"
                            select dt).ToList();

            docno = pattern;
            docno = docno.Replace("Pre", patterns.FirstOrDefault(x => x.DocCode == "[Pre]").DocValue);
            docno = docno.Replace("#", "") + runNumber.ToString("D" + patterns.FirstOrDefault(x => x.DocCode == "[#]").DocValue);
            return docno;
        }

        public async Task<LogResource> SaveListAsync(List<SalCreditsaleHd> invoicelist)
        {
            LogResource result = new LogResource();
            try
            {
                if (invoicelist == null || invoicelist.Count() == 0)
                {
                    throw new Exception("ไม่พบรายการใบแจ้งหนี้");
                }

                SalCreditsaleHd creditsale = invoicelist.First();
                var docpattern = this.Context.MasDocPatterns.FirstOrDefault(x => x.DocType == "Invoice");
                var pattern = (docpattern == null) ? "" : docpattern.Pattern;
                pattern = pattern.Replace("Brn", creditsale.BrnCode);
                pattern = pattern.Replace("yy", creditsale.DocDate.Value.ToString("yy"));
                pattern = pattern.Replace("MM", creditsale.DocDate.Value.ToString("MM"));
                int runNumber = GetRunNumber(creditsale.CompCode, creditsale.BrnCode, pattern);


                using (var scope = serviceScopeFactory.CreateScope())
                {

                    foreach (var head in invoicelist)
                    {
                        head.RunNumber = ++runNumber;
                        var docNo = GenDocNo(head, pattern, (int)head.RunNumber);
                        head.DocType = "Invoice";
                        head.DocNo = docNo;
                        head.DocStatus = "Active";
                        head.DocPattern = pattern;
                        head.ItemCount = head.SalCreditsaleDt.Count();
                        head.Post = "N";
                        head.TxNo = "";
                        head.CreatedBy = "Transfer";
                        head.CreatedDate = DateTime.Now;
                        head.UpdatedBy = head.UpdatedBy;
                        head.UpdatedDate = DateTime.Now;



                        await _invoiceRepository.AddHdAsync(head);
                        int seq = 0;
                        foreach (var dt in head.SalCreditsaleDt)
                        {
                            dt.CompCode = head.CompCode;
                            dt.BrnCode = head.BrnCode;
                            dt.LocCode = head.LocCode;
                            dt.DocType = head.DocType;
                            dt.DocNo = docNo;
                            dt.SeqNo = ++seq;
                            await _invoiceRepository.AddDtAsync(dt);
                        }

                        //Reject ใบซ้ำ
                        //bool duplicateDoc = _invoiceRepository.CheckExistsRefNo(head);
                        //if (!duplicateDoc)
                        //{

                        //}

                    }

                    #region CreateLog
                    var log = new SalCreditsaleLog();
                    //log.LogNo = (this.Context.SalCreditsaleLogs.Max(e => (int?)e.LogNo) ?? 0) + 1;
                    log.LogStatus = "Create";
                    log.CompCode = creditsale.CompCode;
                    log.BrnCode = creditsale.BrnCode;
                    log.LocCode = creditsale.LocCode;
                    log.DocNo = creditsale.DocNo;
                    log.RefNo = creditsale.RefNo??"";
                    log.JsonData = JsonConvert.SerializeObject(invoicelist);
                    log.CreatedBy = "TransferData";
                    log.CreatedDate = DateTime.Now;
                    await _invoiceRepository.AddLogAsync(log);
                    result.LogNo = log.LogNo;
                    #endregion


                    await _unitOfWork.CompleteAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred when saving the InvoiceHd: {ex.Message}");
            }
        }
    }
}
