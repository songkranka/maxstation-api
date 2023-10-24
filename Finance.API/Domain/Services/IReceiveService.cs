using Finance.API.Domain.Models;
using Finance.API.Domain.Models.Queries;
using Finance.API.Resources.Recive;
using MaxStation.Entities.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Domain.Services
{
    public interface IReceiveService
    {
        Task<QueryResult<FinReceiveHd>> ListAsync(ReceiveHdQuery query);
        Task<FinReceiveHd> FindByIdAsync(ReceiveHdQuery query);
        Task<ReceiveHdResponse> SaveAsync(FinReceiveHd obj);
        Task<ReceiveHdResponse> UpdateAsync(FinReceiveHd obj);
        Task<List<FinReceivePay>> GetRemainFinBalanceList(ReceiveQuery query);
        Task<FinReceivePay[]> GetFinReceivePays(FinReceiveHd pInput);
        Task<MasMapping[]> GetMasMapping();
        string GetErrorMessage(Exception pException);
        Task<ModelSumRecivePayResult[]> SumReceivePay(string pStrComCode, string pStrBrnCode, DateTime pDatDocDate);

    }
}
