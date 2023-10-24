using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface ICompanyRepository
    {
        Task<List<MasCompany>> GetAll();
        Task<MasCompany> FindCustomerCompanyById(string guid);
    }
}
