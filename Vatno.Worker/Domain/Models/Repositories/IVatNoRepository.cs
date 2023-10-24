using Vatno.Worker.Domain.Models.response;

namespace Vatno.Worker.Domain.Models.Repositories
{
    public interface IVatNoRepository
    {
        Task<List<VatNoResponse>> VatNoAsync();
    }
}