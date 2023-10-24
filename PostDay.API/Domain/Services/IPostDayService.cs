using PostDay.API.Domain.Models.PostDay;
using PostDay.API.Domain.Models.RestAPI;
using PostDay.API.Domain.Services.Communication.PostDay;
using PostDay.API.Resources.PostDay;
using System;
using System.Data;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Services
{
    public interface IPostDayService
    {
        Task<SaveDocumentResponse> SaveDocument(SaveDocumentRequest req);        
        Task<SaveDocumentResponse> CreateTaxInvoice(PostDayResource req);
        Task<PostDayResponse> GetDocument(GetDocumentRequest req);
        Task<int> AddStock(AddStockParam param);
        Task<DateTime> TestSelectDate();
        Task<DateTime?> TestSelectDate2();
        Task<int> AddStockMonthly(AddStockMonthlyParam param);
        Task<DataTable> GetDopValidData(GetDopValidDataParam param);
    }
}
