using DailyOperation.API.Domain.Models;
using DailyOperation.API.Domain.Models.Queries;
using DailyOperation.API.Domain.Services.Communication;
using DailyOperation.API.Resources.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaxStation.Entities.Models;
using DailyOperation.API.Domain.Models.Request;
using DailyOperation.API.Domain.Models.Response;

namespace DailyOperation.API.Domain.Services
{
    public interface IPosService
    {
        QueryResult<POSCash> CashList(CashQueryResource query);
        QueryResult<POSCredit> CreditList(CreditQueryResource query);
        QueryResult<POSWithdraw> WithdrawList(WithdrawQueryResource query);
        QueryResult<POSReceive> ReceiveList(ReceiveQueryResource query);
        Task<CashsaleResponse> SaveCashSaleAsync(SaveCashSaleResource request);
        Task<CreditsaleResponse> SaveCreditSaleAsync(SaveCreditSaleResource request);
        Task<WithdrawResponse> SaveWithdrawAsync(SaveWithdrawResource request);
        Task<ReceiveResponse> SaveReceiveAsync(SaveReceiveResource request);
        DopPosConfig GetWithdrawStatus(WithdrawStatusRequest req);
        CreditSummaryResponse GetCreditSummaryByBranch(CreditSummaryRequest query);
        Task<PeriodCountResponse> GetPeriodCount(PeriodCountRequest query);
        Task<CheckPeriodWaterResponse> CheckPeriodWater(PeriodCountRequest query);
        string GetConn();

        Task<DopPosConfig[]> GetDopPosConfig(GetDopPosConfigParam param);
    }
}
