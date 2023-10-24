using Finance.API.Domain.Models.Queries;
using Finance.API.Domain.Models.Request;
using Finance.API.Domain.Models.Response;
using Finance.API.Models;
using Finance.API.Resources.Expense;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Finance.API.Domain.Services
{
    public interface IExpenseService
    {
        Task<QueryResult<FinExpenseHd>> ListAsync(ExpenseQuery query);
        Task<string> GetMasDocPattern(string compCode, string brnCode, string docType, string docDate);
        Task<List<ExpenseTable>> GetMasExpenseTableAsync(string status, string compCode, string brnCode, string locCode, string docNo);
        Task<FinExpenseHd> GetExpenseHdAsync(string compCode, string brnCode, string locCode, Guid docNo);
        Task<List<ExpenseEssTable>> GetExpenseEssTableAsync(string compCode, string brnCode, string locCode, string guId);
        Task<SaveExpenseResponse> SaveExpenseHdAsync(SaveExpenseRequest request);
        Task<UpdateStatusExpenseResponse> UpdateExpenseHdStatusAsync(FinExpenseHd request);
    }
}
