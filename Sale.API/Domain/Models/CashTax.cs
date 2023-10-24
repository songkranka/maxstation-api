using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Models
{
    public class CashTax : SalTaxinvoiceHd
    {
        public List<SalTaxinvoiceDt> SalTaxinvoiceDt { get; set; }
    }
}
