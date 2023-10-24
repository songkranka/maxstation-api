using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Services.Communication
{
    public class CashTaxHdResponse : BaseResponse<SalTaxinvoiceHd>
    {
        public CashTaxHdResponse(SalTaxinvoiceHd taxinvoiceHd) : base(taxinvoiceHd) { }

        public CashTaxHdResponse(string message) : base(message) { }
    }
}
