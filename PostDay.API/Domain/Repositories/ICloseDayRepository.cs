using MaxStation.Entities.Models;
using PostDay.API.Domain.Models;
using PostDay.API.Domain.Models.PostDay;
using PostDay.API.Domain.Models.Response;
using PostDay.API.Resources.PostDay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Repositories
{
    public interface ICloseDayRepository
    {
        Task<GetCloseDayResponse> GetTransactionCloseday(GetDocumentRequest req);
        Task<GetCloseDayResponse> GetDocument(GetDocumentRequest req);
        Task AddDopPostdayAsync(SaveDocumentRequest request);
        Task AddDopPostdayValidateAsync(List<DopPostdayValidate> dopPostdayValidates);
        Task UpdatePeriodAsync(PostDayResource postDayResource);
        Task UpdateRequestAsync(PostDayResource postDayResource);
        Task UpdateReceiveAsync(PostDayResource postDayResource);
        Task UpdateTranferOutAsync(PostDayResource postDayResource);
        Task UpdateTranferInAsync(PostDayResource postDayResource);
        Task UpdateWithdrawAsync(PostDayResource postDayResource);
        Task UpdateReturnSupAsync(PostDayResource postDayResource);
        Task UpdateAuditAsync(PostDayResource postDayResource);
        Task UpdateAdjustAsync(PostDayResource postDayResource);
        Task UpdateAdjustRequestAsync(PostDayResource postDayResource);
        Task UpdateReturnOilAsync(PostDayResource postDayResource);
        Task UpdateUnusableAsync(PostDayResource postDayResource);
        Task UpdateQuotationAsync(PostDayResource postDayResource);
        Task UpdateCashSaleAsync(PostDayResource postDayResource);
        Task UpdateCreditSaleAsync(PostDayResource postDayResource);
        Task UpdateTaxinvoiceAsync(PostDayResource postDayResource);
        Task UpdateSalCndnAsync(PostDayResource postDayResource);
        Task UpdateBillingAsync(PostDayResource postDayResource);
        Task UpdateFinanceReceiveAsync(PostDayResource postDayResource);
        Task UpdateBranchConfigAsync(PostDayResource postDayResource);
        Task UpdateMasControlAsync(PostDayResource postDayResource);
        MasDocPattern GetDocPattern(string docType);
        int GetRunningNo(string compCode, string brnCode, string pattern);
        //Task CreateCreditSaleAsync(PostDayResource postDayResource, string pattern, int runno);
        Task CreateTaxInvoiceAsync(PostDayResource postDayResource, string pattern, int runno);
        Task<RequestWarpadModel> GetDataToWarpadAsync(PostDayResource postDayResource);
        Task SendDataToWarpadAsync(RequestWarpadModel warpadData);
        Task ExecuteStoredprocedureStockAsnyc(SaveDocumentRequest saveDocumentRequest, DateTime docDate);
        Task<DopPostdayHd> GetDopPostDayHd(GetDocumentRequest req, DateTime docDate);
        Task<List<DopPostdayDt>> GetDopPostDayDt(GetDocumentRequest req, DateTime docDate);
        Task<List<Formula>> GetFormula(GetDocumentRequest req, DateTime docDate);
        Task<SumInDay> GetSumDate(GetDocumentRequest req, DateTime docDate);
        Task<CheckBeforeSaveView> CheckBeforeSave(GetDocumentRequest req, DateTime docDate);
    }
}
