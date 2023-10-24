using MasterData.API.Resources.Company;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface ICompanyService
    {
        Task<List<MasCompany>> GetAll();
        Task<MasCompany> FindCustomerCompanyById(CustomerCompanyQuery query);
    }
}
