using MaxStation.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;
using Transferdata.API.Domain.Services.Communication;
using Transferdata.API.Resources;
using Transferdata.API.Resources.Transferdata;


namespace Transferdata.API.Services
{
    public class TransferdataService : ITransferdataService
    {
        private readonly ITransferdataRepository _transferdataRepository;
        private readonly IWarpadRepository _warpadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private  PTMaxstationContext Context;

        public TransferdataService(
            ITransferdataRepository transferdataRepository,
            IWarpadRepository warpadRepository,
            IUnitOfWork unitOfWork,
            PTMaxstationContext context,
            IServiceScopeFactory serviceScopeFactory)
        {
            _transferdataRepository = transferdataRepository;
            _warpadRepository = warpadRepository;
            _unitOfWork = unitOfWork;
            Context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }


        public async Task<SalCreditsaleHdResponse> SaveCreditSaleAsync(IEnumerable<SalCreditsaleHd> salCreditsaleHds)
        {
            try
            {
                await _transferdataRepository.AddCreditSaleAsync(salCreditsaleHds);
                await _unitOfWork.CompleteAsync();

                return new SalCreditsaleHdResponse(salCreditsaleHds.LastOrDefault());
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new SalCreditsaleHdResponse($"An error occurred when saving the cashSaleHd: {ex.InnerException}");
            }
        }


        public async Task<SalCashsaleHdResponse> SaveCashSaleAsync(IEnumerable<SalCashsaleHd> salCashsaleHds)
        {
            try
            {
                await _transferdataRepository.AddCashsaleAsync(salCashsaleHds);
                await _unitOfWork.CompleteAsync();

                return new SalCashsaleHdResponse(salCashsaleHds.LastOrDefault());
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new SalCashsaleHdResponse($"An error occurred when saving the cashSaleHd: {ex.InnerException}");
            }
        }

        #region Get Method
        public async Task<List<SalCashsaleHd>> ListCashSaleAsync(TransferDataResource query)
        {
            return await _transferdataRepository.ListCashSaleAsync(query);
        }

        public async Task<List<SalCreditsaleHd>> ListCreditSaleAsync(TransferDataResource query)
        {
            return await _transferdataRepository.ListCreditSaleAsync(query);
        }
        #endregion

        #region Update Method
        public async Task<LogResource> UpdateCloseDayAsync(TransferDataResource query)
        {
            LogResource log = new LogResource();
            using (var tran = this.Context.Database.BeginTransaction())
            {
                try
                {

                    log = await _transferdataRepository.UpdateCloseDayAsync(query);
                    this.Context.SaveChanges();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw new Exception(ex.InnerException.Message);
                }
            }
            await _warpadRepository.SendCloseDay(query);//send warpad
            return log;
        }

        public async Task CraateTaxInvoiceAsync(TransferDataResource query)
        {
            await _transferdataRepository.CreateTaxInvoiceAsync(query);
            await _unitOfWork.CompleteAsync();
        }


        #endregion


    }
}
