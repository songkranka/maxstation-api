using MaxStation.Entities.Models;
using System.Threading.Tasks;

namespace Transferdata.API.Domain.Repositories
{
    public interface IInvoiceRepository
    {
        Task AddHdAsync(SalCreditsaleHd creditSaleHd);
        Task AddDtAsync(SalCreditsaleDt creditSaleDt);

        Task AddLogAsync(SalCreditsaleLog creditSaleLog);
        bool CheckExistsRefNo(SalCreditsaleHd obj);
    }
}
