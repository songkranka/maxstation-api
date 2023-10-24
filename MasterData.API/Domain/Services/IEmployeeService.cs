using MasterData.API.Domain.Models.Queries;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IEmployeeService
    {
        Task<MasEmployee> FindByIdAsync(string id);
        Task<QueryResult<MasEmployee>> ListAsync(EmployeeQuery query);
        Task<QueryResult<MasEmployee>> ListAllWitnoutPageAsync(EmployeeQuery query);
        Task<QueryResult<MasEmployee>> ListAllByBranch(EmployeeQueryByBranch query);
    }
}
