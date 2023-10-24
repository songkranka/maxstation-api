using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class DropdownService : IDropdownService
    {
        private readonly ICompanyCarRepository _companyCarRepository;
        private readonly IBranchRepository _branchRepository;

        public DropdownService(
            ICompanyCarRepository companyCarRepository,
            IBranchRepository branchRepository)
        {
            _companyCarRepository = companyCarRepository;
            _branchRepository = branchRepository;
        }

        public List<MasCompanyCar> GetCompanyCarLicenseList(LicensePlateRequest req)
        {
            return _companyCarRepository.GetCompanyCarLicenseList(req);
        }

        public List<MasBranch> GetBranchList(BranchDropdownRequest req)
        {
            return _branchRepository.GetBranchDropdownList(req);
        }
    }
}
