using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MasEmployee> FindByIdAsync(string id)
        {
            return await _employeeRepository.FindByIdAsync(id);
        }

        public async Task<QueryResult<MasEmployee>> ListAllWitnoutPageAsync(EmployeeQuery query)
        {
            return await _employeeRepository.ListAllWitnoutPageAsync(query);
        }

        public async Task<QueryResult<MasEmployee>> ListAllByBranch(EmployeeQueryByBranch query)
        {
            return await _employeeRepository.ListAllByBranch(query);
        }

        public async Task<QueryResult<MasEmployee>> ListAsync(EmployeeQuery query)
        {
            return await _employeeRepository.ListAsync(query);
        }
    }
}
