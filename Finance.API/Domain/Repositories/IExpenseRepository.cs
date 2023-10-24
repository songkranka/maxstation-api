using Finance.API.Domain.Models.Queries;
using Finance.API.Domain.Models.Request;
using Finance.API.Domain.Models.Response;
using Finance.API.Models;
using Finance.API.Resources.Expense;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Finance.API.Domain.Repositories
{
    public interface IExpenseRepository
    {
        Task<QueryResult<FinExpenseHd>> ListAsync(ExpenseQuery query);
        Task<List<ExpenseTable>> GetMasExpenseTableAsync(string status, string compCode, string brnCode, string locCode, string docNo);
        Task<FinExpenseHd> GetExpenseHdAsync(string compCode, string brnCode, string locCode, Guid guid);
        Task<FinExpenseHd> GetExpenseHdByDocNoAsync(string compCode, string brnCode, string locCode, string docNo);
        Task<FinExpenseHd> GetExpenseHdByDocDateAsync(string compCode, string brnCode, string locCode, DateTime docDate);
        Task<List<ExpenseEssTable>> GetExpenseEssTableAsync(string compCode, string brnCode, string locCode, string docNo);
        Task<List<FinExpenseDt>> GetExpenseDtByDocNoAsync(string compCode, string brnCode, string locCode, string docNo);
        Task<List<FinExpenseEss>> GetExpenseEssByDocNoAsync(string compCode, string brnCode, string locCode, string docNo);
        Task<int?> GetExpenseHdMaxRunNumber(string compCode, string brnCode, string locCode, DateTime docDate);
        Task<int> CountExpenseHd(string compCode, string brnCode, string locCode, DateTime docDate);
        Task AddExpenseHdAsync(SaveFinExpenseHd expenseHd);
        Task AddExpenseDtRangeAsync(List<FinExpenseDt> finExpenseDt);
        Task AddExpenseEssRangeAsync(List<FinExpenseEss> finExpenseEss);
        void UpdateExpenseHd(FinExpenseHd finExpenseHd);
        void RemoveRangeExpenseDt(IEnumerable<FinExpenseDt> finExpenseDts);
        void RemoveRangeExpenseEss(IEnumerable<FinExpenseEss> finExpenseEsss);
        void UpdateStatus(FinExpenseHd finExpenseHd);
    }
}
