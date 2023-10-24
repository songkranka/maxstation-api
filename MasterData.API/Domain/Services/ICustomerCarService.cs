using MasterData.API.Domain.Models.Dropdowns;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Resources.CustomerCar;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface ICustomerCarService
    {
        QueryResult<CustomerCar> CustomerCarDropdownList(Resources.CustomerCar.CustomerCarQuery query);
        Task<QueryResult<MasCustomerCar>> FindByCustCodeAsync(Models.Queries.CustomerCarQuery query);
        Task<List<MasCustomerCar>> GetAllByCompany();
    }
}
