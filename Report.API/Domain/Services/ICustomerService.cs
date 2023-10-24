using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Report.API.Domain.Services
{
    public interface ICustomerService
    {
        List<Debtor02Response> GetDebtor02PDF(CustomerRequest req);
        Task<MemoryStream> GetDebtor02ExcelAsync(CustomerRequest req);
        Task<MemoryStream> ExportExcelAsync(ExportExcelRequest request);
    }

}
