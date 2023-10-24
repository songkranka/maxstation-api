using MasterData.API.Domain.Models.Request;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IDropdownService
    {
        List<MasCompanyCar> GetCompanyCarLicenseList(LicensePlateRequest req);
        List<MasBranch> GetBranchList(BranchDropdownRequest req);
    }
}
