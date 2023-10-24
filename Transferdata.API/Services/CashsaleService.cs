using MaxStation.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;
using Transferdata.API.Resources;
using Transferdata.API.Resources.CashSale;

namespace Transferdata.API.Services
{
    public class CashsaleService : ICashSaleService
    {
        private readonly ICashsaleRepository _cashsaleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private PTMaxstationContext Context;

        public CashsaleService(
            ICashsaleRepository cashsaleRepository,
            IUnitOfWork unitOfWork,
            PTMaxstationContext context,
            IServiceScopeFactory serviceScopeFactory)
        {
            _cashsaleRepository = cashsaleRepository;
            _unitOfWork = unitOfWork;
            Context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<List<SalCashsaleHd>> ListCashSaleAsync(CashsaleQuery query)
        {
            return await this._cashsaleRepository.ListCashsaleAsync(query);
        }

        protected int GetRunNumber(string comp_code, string brn_code, string pattern)
        {
            int runNumber = 0;            
            var resp = this.Context.SalCashsaleHds.Where(x => x.CompCode == comp_code && x.BrnCode == brn_code && x.DocPattern == pattern).OrderByDescending(x => x.RunNumber).FirstOrDefault();
            if (resp != null)
            {
                runNumber = (resp.RunNumber ?? 0);
            }
            return runNumber;
        }

        protected string GenDocNo(SalCashsaleHd query, string pattern, int runNumber)
        {
            string docno = "";

            var date = query.DocDate.Value;
            var Brn = query.BrnCode;

            var patterns = (from hd in this.Context.MasDocPatterns
                            join dt in this.Context.MasDocPatternDts on hd.DocId equals dt.DocId
                            where hd.DocType == "CashSale"
                            select dt).ToList();

            docno = pattern;
            docno = docno.Replace("Pre", patterns.FirstOrDefault(x => x.DocCode == "[Pre]").DocValue);
            docno = docno.Replace("#", "") + runNumber.ToString("D" + patterns.FirstOrDefault(x => x.DocCode == "[#]").DocValue);
            return docno;
        }


        public async Task<LogResource> SaveListAsync(List<SalCashsaleHd> cashsalelist)
        {
            LogResource result = new LogResource();
            try
            {
                if(cashsalelist == null || cashsalelist.Count() == 0)
                {
                    throw new Exception("ไม่พบรายการขายสดจาก POS");
                }

                SalCashsaleHd cashsale = cashsalelist.First();
                var docpattern = this.Context.MasDocPatterns.FirstOrDefault(x => x.DocType == "CashSale");
                var pattern = (docpattern == null) ? "" : docpattern.Pattern;
                pattern = pattern.Replace("Brn", cashsale.BrnCode);
                pattern = pattern.Replace("yy", cashsale.DocDate.Value.ToString("yy"));
                pattern = pattern.Replace("MM", cashsale.DocDate.Value.ToString("MM"));
                int runNumber = GetRunNumber(cashsale.CompCode, cashsale.BrnCode, pattern);


                using (var scope = serviceScopeFactory.CreateScope())
                {
                    
                    foreach (SalCashsaleHd head in cashsalelist)
                    {
                        head.RunNumber = ++runNumber;
                        var docNo = GenDocNo(head, pattern, (int)head.RunNumber);
                        head.DocNo = docNo;
                        head.DocStatus = "Active";
                        head.DocPattern = pattern;
                        head.ItemCount = head.SalCashsaleDt.Count();
                        head.Post = "N";
                        head.CreatedBy = "POS";                        
                        head.CreatedDate = DateTime.Now;
                        head.UpdatedBy = "POS";
                        head.UpdatedDate = DateTime.Now;

                        //Reject ใบซ้ำ
                        bool duplicateDoc =  _cashsaleRepository.CheckExistsPOS(head);
                        if (!duplicateDoc)
                        {
                             await _cashsaleRepository.AddHdAsync(head);
                            int seq = 0;
                            foreach (SalCashsaleDt dt in head.SalCashsaleDt)
                            {
                                dt.DocNo = docNo;
                                dt.SeqNo = ++seq;                                
                                await _cashsaleRepository.AddDtAsync(dt);
                            }                            
                        }
                       
                    }

                    #region CreateLog
                    var cashsalelog = new SalCashsaleLog();
                    cashsalelog.LogStatus = "Create";
                    cashsalelog.CompCode = cashsale.CompCode;
                    cashsalelog.BrnCode = cashsale.BrnCode;
                    cashsalelog.LocCode = cashsale.LocCode;
                    cashsalelog.DocNo = cashsale.DocNo;
                    cashsalelog.JsonData = JsonConvert.SerializeObject(cashsalelist);
                    cashsalelog.CreatedBy = "TransferData";
                    cashsalelog.CreatedDate = DateTime.Now;                    
                    await _cashsaleRepository.AddLogAsync(cashsalelog);
                    result.LogNo = cashsalelog.LogNo;
                    #endregion


                    await _unitOfWork.CompleteAsync();
                }


               
                return result;
            }
            catch (Exception ex)
            {                
                throw new Exception($"An error occurred when saving the cashSaleHd: {ex.Message}");
            }
        }
    }
}
