using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;
using Transferdata.API.Resources;

namespace Transferdata.API.Services
{
    public class CreditsaleService : ICreditSaleService
    {
        private readonly ICreditsaleRepository _creditsaleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private PTMaxstationContext Context;

        public CreditsaleService(
            ICreditsaleRepository creditsaleRepository,
            IUnitOfWork unitOfWork,
            PTMaxstationContext context,
            IServiceScopeFactory serviceScopeFactory)
        {
            _creditsaleRepository = creditsaleRepository;
            _unitOfWork = unitOfWork;
            Context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<List<SalCreditsaleHd>> ListCrditSaleAsync(CreditsaleQuery query)
        {
            return await this._creditsaleRepository.ListCreditsaleAsync(query);
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
                            where hd.DocType == "CreditSale"
                            select dt).ToList();

            docno = pattern;
            docno = docno.Replace("Pre", patterns.FirstOrDefault(x => x.DocCode == "[Pre]").DocValue);
            docno = docno.Replace("#", "") + runNumber.ToString("D" + patterns.FirstOrDefault(x => x.DocCode == "[#]").DocValue);
            return docno;
        }

        public async Task<LogResource> SaveListAsync(List<SalCreditsaleHd> creditsalelist)
        {
            LogResource result = new LogResource();
            try
            {
                if (creditsalelist == null || creditsalelist.Count() == 0)
                {
                    throw new Exception("ไม่พบรายการขายเชื่อจาก POS");
                }

                SalCreditsaleHd creditsale = creditsalelist.First();
                var docpattern = this.Context.MasDocPatterns.FirstOrDefault(x => x.DocType == "CreditSale");
                var pattern = (docpattern == null) ? "" : docpattern.Pattern;
                pattern = pattern.Replace("Brn", creditsale.BrnCode);
                pattern = pattern.Replace("yy", creditsale.DocDate.Value.ToString("yy"));
                pattern = pattern.Replace("MM", creditsale.DocDate.Value.ToString("MM"));
                int runNumber = GetRunNumber(creditsale.CompCode, creditsale.BrnCode, pattern);


                using (var scope = serviceScopeFactory.CreateScope())
                {

                    foreach (var head in creditsalelist)
                    {
                        //head.RunNumber = ++runNumber;
                        //var docNo = GenDocNo(head, pattern, (int)head.RunNumber);
                        string docNo = string.Empty;
                        do
                        {
                            docNo = GenDocNo(head, pattern, ++runNumber);
                        } while (await IsDupplicateDocNo(head, docNo));
                        head.RunNumber = runNumber;
                        head.DocType = "CreditSale";
                        head.DocNo = docNo;
                        head.DocStatus = "Active";
                        head.DocPattern = pattern;
                        head.ItemCount = head.SalCreditsaleDt.Count();
                        head.Post = "N";
                        head.TxNo = "";
                        head.CreatedBy = "POS";
                        head.CreatedDate = DateTime.Now;
                        head.UpdatedBy = "POS";
                        head.UpdatedDate = DateTime.Now;

                        //Reject ใบซ้ำ
                        bool duplicateDoc = _creditsaleRepository.CheckExistsPOS(head);
                        if (!duplicateDoc)
                        {
                            await _creditsaleRepository.AddHdAsync(head);
                            int seq = 0;
                            foreach (var dt in head.SalCreditsaleDt)
                            {
                                dt.CompCode = head.CompCode;
                                dt.BrnCode = head.BrnCode;
                                dt.LocCode = head.LocCode;
                                dt.DocType = head.DocType;
                                dt.DocNo = docNo;
                                dt.SeqNo = ++seq;
                                await _creditsaleRepository.AddDtAsync(dt);
                            }
                        }

                    }
                    
                    #region CreateLog
                    var log = new SalCreditsaleLog();
                    //log.LogNo = (this.Context.SalCreditsaleLogs.Max(e => (int?)e.LogNo) ?? 0) + 1;
                    log.LogStatus = "Create";
                    log.CompCode = creditsale.CompCode;
                    log.BrnCode = creditsale.BrnCode;
                    log.LocCode = creditsale.LocCode;
                    log.DocNo = creditsale.DocNo;
                    log.RefNo = creditsale.PosNo;
                    log.JsonData = JsonConvert.SerializeObject(creditsalelist);
                    log.CreatedBy = "TransferData";
                    log.CreatedDate = DateTime.Now;
                    await _creditsaleRepository.AddLogAsync(log);
                    result.LogNo = log.LogNo;
                    #endregion


                    await _unitOfWork.CompleteAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred when saving the CreditSaleHd: {ex.Message}");
            }
        }

        public async Task<bool> IsDupplicateDocNo(SalCreditsaleHd pHeader, string pStrDocNo)
        {
            if (pHeader == null)
            {
                return false;
            }
            //Context.SalCreditsaleHds
            var qryCreditSale = Context.SalCreditsaleHds.Where(
                x => x.DocNo == pStrDocNo
                && x.CompCode == pHeader.CompCode
                && x.BrnCode == pHeader.BrnCode
                && x.LocCode == pHeader.LocCode
            ).AsNoTracking();
            var result = await qryCreditSale.AnyAsync();
            return result;

        }


    }
}
