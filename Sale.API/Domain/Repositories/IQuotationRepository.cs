using MaxStation.Entities.Models;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Resources;
using Sale.API.Resources.Quotation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sale.API.Domain.Repositories
{
    public interface IQuotationRepository
    {
        void CreateQuotation(SalQuotationHd obj);
        void DeleteQuotationDT(SalQuotationHd obj);
        string GetMaxCardProfile(string pStrMaxcardId);
        Task<SalQuotationHd> GetQuotation(RequestData req);
        Task<QueryResult<SalQuotationHd>> GetQuotationHdList(QuotationHdQuery req);
        Task<QueryResult<QuotationResource>> SearchList(QuotationHdQuery req);
        int GetQuotationHdCount(RequestData req);
        Task<List<SalQuotationHd>> GetQuotationHdRemainList(RequestData req);
        int GetRunNumber(SalQuotationHd obj);
        void UpdateQuotation(SalQuotationHd obj);
        void UpdateDocStatusQuotation(SalQuotationHd obj);
        public Task<MasPayType[]> GetArrayPayType();
        public Task<MasEmployee[]> GetArrayEmployee();
        public Task<MasEmployee> GetEmployee(string pStrEmpCode);
        Task CreatApprove(SalQuotationHd param);
        Task CancelApprove(SalQuotationHd param);
        Task<SysApproveStep[]> GetApproveStep(SalQuotationHd param);
    }
}
