using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Report.API.Domain.Services
{
    public interface IReceivePayService
    {
       // MemoryStream GetReceivePayExcel(ReportSummarySaleRequest req);
        Task<ReceivePayResponse> GetReceivePayPDF(ReceivePayRequest req);
    }


}
