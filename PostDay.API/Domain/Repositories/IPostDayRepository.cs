
using PostDay.API.Domain.Models.PostDay;
using PostDay.API.Domain.Models.RestAPI;
using PostDay.API.Domain.Services.Communication.PostDay;
using PostDay.API.Resources.PostDay;
using System;
using System.Data;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Repositories
{
    public interface IPostDayRepository
    {
        Task<SaveDocumentResponse> SaveDocument(SaveDocumentRequest req);
        Task UpdateCreditSaleAsync(PostDayResource req);
        Task CreateTaxInvoiceAsync(PostDayResource req);
        
        Task<PostDayResponse> GetDocument(GetDocumentRequest req);
        Task<DataTable> GetDopValidData(GetDopValidDataParam param);
    }
}
