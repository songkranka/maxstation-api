using MasterData.API.Domain.Models.Responses.Price;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class PriceService : IPriceService
    {
        private readonly IPriceRepository _priceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PriceService(
            IPriceRepository priceRepository,
            IUnitOfWork unitOfWork)
        {
            _priceRepository = priceRepository;
            _unitOfWork = unitOfWork;
        }
        public List<OilPrice> GetCurrentOilPriceAsync(string compCode, string brnCode, DateTime systemDate)
        {
            return  _priceRepository.GetCurrentOilPriceAsync(compCode, brnCode, systemDate);
        }
    }
}
