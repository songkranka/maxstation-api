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
    public class CompanyCarService : ICompanyCarService
    {
        private readonly ICompanyCarRepository _companyCarRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CompanyCarService(
            ICompanyCarRepository companyCarRepository,
            IUnitOfWork unitOfWork)
        {
            _companyCarRepository = companyCarRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<QueryResult<MasCompanyCar>> ListAsync(CompanyCarQuery query)
        {
            return await _companyCarRepository.ListAsync(query);
        }
    }
}
