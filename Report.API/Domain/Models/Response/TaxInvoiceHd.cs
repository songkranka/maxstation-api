using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{

    public class TaxInvoiceHd
    {
        public string comp_code { get; set; }
        public string company_name { get; set; }
        public string company_address { get; set; }
        public string company_phone { get; set; }
        public string company_fax { get; set; }
        public string company_image { get; set; }
        public string company_cust_code { get; set; }
        public string company_citizenid { get; set; }
        public string brn_code { get; set; }
        public string brn_name { get; set; }
        public string brn_address { get; set; }
        public string brn_no { get; set; }
        public string loc_code { get; set; }
        public string doc_no { get; set; }
        public string doc_date { get; set; }
        public string cust_code { get; set; }
        public string cust_name { get; set; }
        public string cust_addr1 { get; set; }
        public string cust_addr2 { get; set; }
        public string citizen_id { get; set; }
        public decimal? sub_amt { get; set; }
        public decimal? disc_amt { get; set; }
        public decimal? vat_amt { get; set; }
        public decimal? net_amt { get; set; }
        public decimal? tax_base_amt { get; set; }
        public string net_amt_text { get; set; }
        public string emp_code { get; set; }
        public string emp_name { get; set; }
        public string license_plate { get; set; }

        public List<TaxInvoiceDt> items = new List<TaxInvoiceDt>();
        
    }

    public class TaxInvoiceDt
    {
        public decimal? item_qty { get; set; }
        public string product_name { get; set; }
        public string unit_name { get; set; }
        public decimal? unit_price { get; set; }
        public decimal? disc_amt { get; set; }
        public decimal? sub_amt { get; set; }
        public int seq_no { get; set; }
        public string license_plate { get; set; }
    }


}
