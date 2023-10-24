using Finance.API.Domain.Repositories;
using Finance.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Repositories
{
    public class TaxInvoiceRepository : SqlDataAccessHelper, ITaxInvoiceRepository
    {
        public TaxInvoiceRepository(PTMaxstationContext context) : base(context)
        {

        }

        public List<SalTaxinvoiceDt> GetTaxInvoiceByDocNo(string docNo)
        {
            return context.SalTaxinvoiceDts.Where(x => x.DocNo == docNo).ToList();
        }

        public SalTaxinvoiceHd GetTaxInvoiceHdByFinReceivePay(FinReceivePay finReceivePay, string custCode)
        {
            return context.SalTaxinvoiceHds.FirstOrDefault(x => x.CompCode == finReceivePay.CompCode 
            && finReceivePay.BrnCode == finReceivePay.BrnCode 
            && x.LocCode == finReceivePay.LocCode 
            && x.DocNo == finReceivePay.TxNo
            && x.CustCode == custCode);
        }
    }
}
