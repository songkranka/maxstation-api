using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Helpers;

namespace Transferdata.API.Repositories
{
    public class TaxInvoiceRepository : SqlDataAccessHelper, ITaxInvoiceRepository
    {
        public TaxInvoiceRepository(PTMaxstationContext context) : base(context)
        {
        }


        public async Task<List<SalTaxinvoiceHd>> ListTaxInvoiceAsync(TaxInvoiceQuery query)
        {
            List<SalTaxinvoiceHd> heads = new List<SalTaxinvoiceHd>();

            heads = await this.context.SalTaxinvoiceHds.Where(x => x.CreatedBy != "dummy"
                                              && x.CompCode == query.CompCode
                                              && x.BrnCode == query.BrnCode
                                              && x.DocDate == query.DocDate
                                            ).ToListAsync();

            heads.ForEach(hd => hd.SalTaxinvoiceDt = this.context.SalTaxinvoiceDts.Where(x => x.CompCode == hd.CompCode
                                                                                 && x.BrnCode == hd.BrnCode
                                                                                 && x.LocCode == hd.LocCode                                                                                 
                                                                                 && x.DocNo == hd.DocNo).ToList()
            );

            return heads;
        }
    }
}
