using MasterData.API.Domain.Models.Queries;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface ICompanyCarService
    {
        Task<QueryResult<MasCompanyCar>> ListAsync(CompanyCarQuery query);
    }
}
