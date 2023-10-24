using Finance.API.Domain.Models;
using Finance.API.Domain.Models.Queries;
using Finance.API.Resources.Recive;
using MaxStation.Entities.Models;
using Sale.API.Domain.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Domain.Repositories
{
    public interface IReceiveRepository
    {
        Task<QueryResult<FinReceiveHd>> ListAsync(ReceiveHdQuery query);
        Task<FinReceiveHd> FindByIdAsync(Guid guid);
        Task AddHdAsync(FinReceiveHd obj);
        Task AddDtAsync(FinReceiveDt obj);
        Task AddPayAsync(FinReceivePay obj);
        void UpdateAsync(FinReceiveHd obj);
        void AddDtListAsync(IEnumerable<FinReceiveDt> listObj);
        void AddPayListAsync(IEnumerable<FinReceivePay> listObj);
        int GetRunNumber(FinReceiveHd hd);
        Task<List<FinReceiveDt>> GetFinReceiveDtByDocNoAsync(string docNo);
        void RemoveDtAsync(IEnumerable<FinReceiveDt> listObj);
        void RemovePayAsync(IEnumerable<FinReceivePay> listObj);
        Task<List<FinReceivePay>> GetFinReceivePayByDocNoAsync(string docNo);
        Task<List<FinReceivePay>> GetRemainFinBalanceList(ReceiveQuery query);
        Task UpdateRemainFinBalance(FinReceiveHd obj);
        Task ReturnRemainFinBalance(FinReceiveHd obj);
        Task<FinReceivePay[]> GetFinReceivePays(FinReceiveHd pInput);
        Task<MasMapping[]> GetMasMapping();
        Task<ModelSumRecivePayResult[]> SumReceivePay(string pStrComCode, string pStrBrnCode, DateTime pDatDocDate);
    }
}
