using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System.IO;
using System.Threading.Tasks;

namespace Report.API.Domain.Repositories
{
    public interface IReceivePayRepository
    {
        MemoryStream GetReceivePayExcel(ReportStockRequest req);
        Task<ReceivePayResponse> GetReceivePayPDF(ReceivePayRequest req);        

    }

}
