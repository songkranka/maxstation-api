using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MasterData.API.Resources.Company;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CompanyService(
            ICompanyRepository companyRepository,
            IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MasCompany>> GetAll()
        {
            return await _companyRepository.GetAll();
        }

        public async Task<MasCompany> FindCustomerCompanyById(CustomerCompanyQuery query)
        {
            return await _companyRepository.FindCustomerCompanyById(query.CustCode);
        }
    }
}
