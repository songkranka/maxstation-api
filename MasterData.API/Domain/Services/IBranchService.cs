using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IBranchService
    {
        List<MasBranch> GetBranchList(BranchRequest req);
        List<MasBranch> GetAuthBranchList(AuthBranchRequest req);
        Task<QueryResult<MasBranch>> List(BranchQuery pQuery);
        List<MasCompany> getCompanyDDL();
        Task<MasCompany> getCompany(string CompCode);
        Task<MasBranch> GetBranchByGuid(Guid guid);
        Task<MasBranch> getBranchDetail(string brnCode);
        Task<MasBranch> GetBranchDetailByCompCodeAndBrnCode(string CompCode, string BrnCode);
        List<MasBranchTank> getTankByBranch(string CompCode, string BrnCode);
        List<MasBranchDisp> getDispByBranch(string CompCode, string BrnCode);
        List<MasBranchTax> getTaxByBranch(string CompCode, string BrnCode);
        List<MasBranch> GetAllBranchByCompCode(string CompCode);
        public Task SaveBranch(MasBranch param);
        Task<QueryResult<MeterResponse>> GetMasBranchDISP(BranchMeterRequest req);
        Task<MasBranchConfig> GetMasBranchConfig(string pStrCompCode, string pStrBrnCode);
        Task<List<MasBranch>> GetBranchEmployeeAuth(BranchEmployeeAuthRequest req);
        Task<SaveBranchResponse> SaveBranchAsync(SaveBranchRequest request);
        Task<SaveBranchTankResponse> SaveBranchTankAsync(SaveBranchTankRequest request); 
        Task<SaveBranchTaxResponse> SaveBranchTaxAsync(SaveBranchTaxRequest request);
    }
}
