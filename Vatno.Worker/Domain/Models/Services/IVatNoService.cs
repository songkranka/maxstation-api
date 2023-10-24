using Vatno.Worker.Domain.Models.response;
using Vatno.Worker.Models;

namespace Vatno.Worker.Domain.Models.Services
{
    public interface IVatNoService
    {
        Task<List<VatNoResponse>> VatExportLog();
        Task<LogVatnoMaxme> SaveLogVatNoAsync(string fileName, string status, string errorMsg);
    }

}
