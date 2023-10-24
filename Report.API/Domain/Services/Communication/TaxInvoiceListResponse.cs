using Report.API.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Services.Communication
{
    public class TaxInvoiceListResponse : BaseResponse<List<TaxInvoice>>
    {
        public TaxInvoiceListResponse(List<TaxInvoice> taxInvoice) : base(taxInvoice) { }

        public TaxInvoiceListResponse(string message) : base(message) { }
    }
}
