using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Services.Communication
{
    public class InvoiceDropdownResponse
    {
        public string PdId { get; set; }
        public string PdName { get; set; }
        public string VatType { get; set; }
        public int? VatRate { get; set; }

    }
}
