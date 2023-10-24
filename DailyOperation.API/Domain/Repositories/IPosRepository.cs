using DailyOperation.API.Domain.Models;
using DailyOperation.API.Domain.Models.Queries;
using DailyOperation.API.Domain.Models.Request;
using DailyOperation.API.Domain.Models.Response;
using DailyOperation.API.Resources.POS;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Repositories
{
    public interface IPosRepository
    {
        QueryResult<POSCash> CashList(CashQueryResource query);
        QueryResult<POSCredit> CreditList(CreditQueryResource query);
        QueryResult<POSWithdraw> WithdrawList(WithdrawQueryResource query);
        QueryResult<POSReceive> ReceiveList(ReceiveQueryResource query);
        Task AddCashSaleListAsync(SaveCashSaleResource request);
        Task AddCreditSaleListAsync(SaveCreditSaleResource request);
        Task AddWithdrawListAsync(SaveWithdrawResource request);
        Task AddReceiveListAsync(SaveReceiveResource request);
        //List<MasBranchMid> GetMasBranchMid(string compCode, string branchCode);
        //MasBranchPeriod GetPeriod(string compCode, string branchCode, int sfihtNo);
        DopPosConfig GetWithdrawStatus(WithdrawStatusRequest req);
        List<string> ValidateCustomer(SaveCreditSaleResource req);
        List<string> ValidateMasProduct(IEnumerable<string> productIds);
        List<string> ValidateMasProductUnit(IEnumerable<string> productIds);
        CreditSummaryResponse GetCreditSummaryByBranch(CreditSummaryRequest request);
        Task<PeriodCountResponse> GetPeriodCount(PeriodCountRequest request);
        Task<int> GetPeriodWaterPOS(PeriodCountRequest query);
        Task<int> GetPeriodWaterMAX(PeriodCountRequest query);
        string GetConn();
        Task<DopPosConfig[]> GetDopPosConfig(GetDopPosConfigParam param);
    }
}
