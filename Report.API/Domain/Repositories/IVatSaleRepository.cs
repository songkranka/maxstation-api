using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;

namespace Report.API.Domain.Repositories
{
    public interface IVatSaleRepository
    {        
        VatSaleResponse GetVatSalePDF(VatSaleRequest req);

    }

}
