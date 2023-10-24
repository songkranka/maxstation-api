
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface IBranchRepository
    {
        List<MasBranch> GetBranchList(BranchRequest req);
        List<MasBranch> GetAuthBranchList(AuthBranchRequest req);
        Task<QueryResult<MasBranch>> List(BranchQuery pQuery);
        Task<MasBranch> GetBranchByGuid(Guid guid);
        Task<MasBranch> getBranchDetail(string brnCode);
        Task<MasBranch> GetBranchDetailByCompCodeAndBrnCode(string CompCode, string BrnCode);
        List<MasCompany> getCompanyDDL();
        Task<MasCompany> getCompany(string CompCode);
        List<MasBranchTank> getTankByBranch(string CompCode, string BrnCode);
        List<MasBranchDisp> getDispByBranch(string CompCode, string BrnCode);
        List<MasBranchTax> getTaxByBranch(string CompCode, string BrnCode);
        List<MasBranch> GetAllBranchByCompCode(string CompCode);
        public void SaveBranch(MasBranch param);
        Task<QueryResult<MeterResponse>> GetMasBranchDISP(BranchMeterRequest req);
        Task<MasBranchConfig> GetMasBranchConfig(string pStrCompCode, string pStrBrnCode);
        List<MasBranch> GetBranchDropdownList(BranchDropdownRequest req);
        MasBranch FindBranchByBrnCode(string brnCode);
        List<MasBranch> FindBranchByCompCode(string compCode);
        List<MasBranch> FindBranchByAuthCode(string compCode,int? authCode);
        Task<MasBranch> FindByGuidAsync(Guid? guid);
        Task AddBranchAsync(MasBranch masBranch);
        Task UpdateBranchAsync(MasBranch masBranch);
        Task<MasBranchTank> FindByCompCodeAndBrnCodeAndTankIdAsync(string compCode, string brnCode, string tankId);
        Task<MasBranchTank> FindByCompCodeAndBrnCodeAsync(string compCode, string brnCode);
        Task AddBranchTankAsync(MasBranchTank masBranch);
        Task UpdateBranchTankAsync(MasBranchTank masBranch);
        Task<MasBranchDisp> FindByCompCodeAndBrnCodeAndDispIdAsync(string compCode, string brnCode, string dispid);
        Task<MasBranchDisp> FindMasBranchDispByCompCodeAndBrnCodeAsync(string compCode, string brnCode);
        Task AddBranchDispAsync(MasBranchDisp masBranch);
        Task UpdateBranchDispAsync(MasBranchDisp masBranch);
        Task AddBranchConfigAsync(MasBranchConfig masBranchConfig);
        Task UpdateBranchConfigAsync(MasBranchConfig masBranchConfig);
        Task<MasBranchTax> FindByCompCodeAndBrnCodeAndTaxIdAsync(string compCode, string brnCode);
        Task AddBranchTaxAsync(MasBranchTax masBranchTax);
        Task UpdateBranchTaxAsync(MasBranchTax masBranchTax);
        Task DeleteBranchTaxAsync(string compCode, string brnCode);
        Task DeleteBranchTankAsync(string compCode, string brnCode);
        Task DeleteBranchDispAsync(string compCode, string brnCode);
    }
}
