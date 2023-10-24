using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Report.API.Domain.Services
{
    public interface ISalesService
    {
        Task<List<Sale02Response>> GetSale02PDF(SalesRequest req);
        Task<MemoryStream> GetSale02ExcelAsync(SalesRequest req);
        List<Sale03Response> GetSale03PDF(SalesRequest req);
        Task<MemoryStream> GetSale03ExcelAsync(SalesRequest req);
        List<Sale04Response> GetSale04PDF(SalesRequest req);
        Task<MemoryStream> GetSale04ExcelAsync(SalesRequest req);

        List<Sale06Response> GetSale06PDF(SalesRequest req);
        Task<MemoryStream> GetSale06ExcelAsync(SalesRequest req);
    }

}
