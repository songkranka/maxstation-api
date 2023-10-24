using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;

namespace Report.API.Domain.Services
{
    public interface IVatSaleService
    {
        VatSaleResponse GetVatSalePDF(VatSaleRequest req);
    }



}
