using MaxStation.Entities.Models;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Resources;
using Sale.API.Resources.Quotation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sale.API.Domain.Services
{
    public interface IQuotationService
    {
        Task<List<SalQuotationDt>> CalculateStock(SalQuotationHd obj);
        Task< SalQuotationHd> CreateQuotation(SalQuotationHd obj);
        string GetMaxCardProfile(string pStrMaxcardId);
        Task<SalQuotationHd> GetQuotation(RequestData req);
        Task<QueryResult<SalQuotationHd>> GetQuotationHDList(QuotationHdQuery req);
        Task<QueryResult<QuotationResource>> SearchList(QuotationHdQuery req);
        Task<List<SalQuotationHd>> GetQuotationHdRemainList(RequestData req);
        Task<SalQuotationHd> UpdateQuotation(SalQuotationHd obj);
        int GetQuotationHdCount(RequestData req);
        public Task<MasPayType[]> GetArrayPayType();
        public Task<MasEmployee[]> GetArrayEmployee();
        public Task<MasEmployee> GetEmployee(string pStrEmpCode);
        Task<SysApproveStep[]> GetApproveStep(SalQuotationHd param);
    }
}
