using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Report.API.Domain.Services
{
    public interface IFinanceService
    {
        List<Fin03Response> GetFin03PDF(FinanceRequest req);
        Task<MemoryStream> GetFin03ExcelAsync(FinanceRequest req);

        List<Fin08Response> GetFin08PDF(FinanceRequest req);
        Task<MemoryStream> GetFin08ExcelAsync(FinanceRequest req);
    }

}
