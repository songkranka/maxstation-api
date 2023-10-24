using Report.API.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Services.Communication
{
    public class TaxInvoiceResponse : BaseResponse<TaxInvoice>
    {
        public TaxInvoiceResponse(TaxInvoice taxInvoice) : base(taxInvoice) { }

        public TaxInvoiceResponse(string message) : base(message) { }
    }
}
