using Common.API.Domain.Models.Queries;
using Common.API.Domain.Repositories;
using Common.API.Domain.Service;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Service
{
    public class CashSaleService : ICashSaleService
    {
        private readonly ICashSaleRepository _cashSaleRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CashSaleService(
            ICashSaleRepository cashSaleRepository,
            IUnitOfWork unitOfWork)
        {
            _cashSaleRepository = cashSaleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<QueryResult<SalCashsaleHd>> ListAsync(CashSaleHdQuery query)
        {            
            GC.SuppressFinalize(this);
            return await _cashSaleRepository.ListAsync(query);
        }
    }
}
