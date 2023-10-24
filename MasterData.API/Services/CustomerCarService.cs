using MasterData.API.Domain.Models.Dropdowns;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MasterData.API.Resources.CustomerCar;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class CustomerCarService : ICustomerCarService
    {
        private readonly ICustomerCarRepository _customerCarRepository;
        public CustomerCarService(
            ICustomerCarRepository customerCarRepository)
        {
            _customerCarRepository = customerCarRepository;
        }

        public QueryResult<CustomerCar> CustomerCarDropdownList(Resources.CustomerCar.CustomerCarQuery query)
        {
            return _customerCarRepository.CustomerCarList(query);
        }

        public async Task<QueryResult<MasCustomerCar>> FindByCustCodeAsync(Domain.Models.Queries.CustomerCarQuery query)
        {
            return await _customerCarRepository.FindByCustCodeAsync(query);
        }

        public async Task<List<MasCustomerCar>> GetAllByCompany()
        {
            return await _customerCarRepository.GetAllByCompany();
        }
    }
}
