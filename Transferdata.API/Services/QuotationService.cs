using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;
using Transferdata.API.Resources;
using Transferdata.API.Resources.Quotation;

namespace Transferdata.API.Services
{

    public class QuotationService : IQuotationService
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private PTMaxstationContext Context;
        readonly string urlMasterAPI = @"https://maxstation-masterdata-api.pt.co.th";

        public QuotationService(
            IQuotationRepository quotationRepository,
            IUnitOfWork unitOfWork,
            PTMaxstationContext context,
            IServiceScopeFactory serviceScopeFactory)
        {
            _quotationRepository = quotationRepository;
            _unitOfWork = unitOfWork;
            Context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }




        public async Task<List<QuotationDtResource>> CalculateStock(QuotationResource request)
        {
            List<QuotationDtResource> resp = new List<QuotationDtResource>();
            try
            {               
                List<MasProductUnit> pdUnit = new List<MasProductUnit>();
                pdUnit = await this.Context.MasProductUnits.Where(x => request.QuotationDt.Select(q => q.UnitBarcode).Contains(x.UnitBarcode) ).ToListAsync();
                resp =  (from req in request.QuotationDt
                        join unit in pdUnit on req.UnitBarcode equals unit.UnitBarcode
                        select new {req,unit }).Select(x => new QuotationDtResource
                                                {
                                                UnitBarcode = x.req.UnitBarcode,
                                                ItemQty = x.req.ItemQty,
                                                ItemQtyBefore = x.req.ItemQtyBefore,
                                                StockQty = x.req.ItemQty * x.unit.UnitStock
                                                }).ToList();
                //x.req.StockQty * x.unit.UnitStock / (x.unit.UnitRatio ?? 1)
                
                return resp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SalQuotationHd> GetQuotationAsync(QuotationResource query)
        {
            return await this._quotationRepository.GetQuotationAsync(query);
        }

        public async Task<List<QuotationMaxCardResource>> ListByMaxCardAsync(QuotationResource query)
        {
            return await this._quotationRepository.ListByMaxCardAsync(query);
        }


        public async Task<SalQuotationHd> UpdateRemainQuotation(QuotationResource obj)
        {
            SalQuotationHd result = new SalQuotationHd();
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    obj.QuotationDt = await this.CalculateStock(obj);
                    result = await _quotationRepository.UpdateRemainQuotation(obj);
                }
                await _unitOfWork.CompleteAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"UpdateRemainQuotation: {ex.Message}");
            }
        }

        public async Task<LogResource> CreateRemainQuotation(QuotationResource obj)
        {
            LogResource result = new LogResource();
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    obj.QuotationDt = await this.CalculateStock(obj);
                    result = await _quotationRepository.CreateRemainQuotation(obj);
                }
                await _unitOfWork.CompleteAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"CreateRemainQuotation : {ex.Message}");
            }
        }

        public async Task<LogResource> CancelRemainQuotation(QuotationResource obj)
        {
            LogResource result = new LogResource();
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {                    
                    SalQuotationLog log = await _quotationRepository.GetLogAsync("Create", obj);

                    QuotationResource logcreate = JsonConvert.DeserializeObject<QuotationResource>(log.JsonData);
                    obj.QuotationDt = logcreate.QuotationDt;
                    result = await _quotationRepository.CancelRemainQuotation(obj);
                }
                await _unitOfWork.CompleteAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
