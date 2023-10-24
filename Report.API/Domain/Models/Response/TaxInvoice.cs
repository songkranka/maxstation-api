using FastReport.Web;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{
    public class TaxInvoice
    {
        public WebReport WebReport { get; set; }
        public SalTaxinvoiceHd SalTaxinvoiceHd { get; set; }
        public List<TaxInvoiceDt> SalTaxinvoiceDt { get; set; }
        public MasCompany MasCompany { get; set; }
        public MasCustomer MasCompanyCustomer { get; set; }
        public MasCustomer MasCustomer { get; set; }
        public MasBranch MasBranch { get; set; }

        public class TaxInvoiceDt
        {
            public decimal? ItemQty { get; set; }
            public string Product { get; set; }
            public string UnitName { get; set; }
            public decimal? UnitPrice { get; set; }
            public decimal? Discount { get; set; }
            public decimal? SubAmtCur { get; set; }
        }
    }



}
