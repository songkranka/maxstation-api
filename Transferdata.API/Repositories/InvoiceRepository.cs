using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Helpers;

namespace Transferdata.API.Repositories
{
    public class InvoiceRepository : SqlDataAccessHelper, IInvoiceRepository
    {
        public InvoiceRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task AddHdAsync(SalCreditsaleHd creditSaleHd)
        {
            await context.SalCreditsaleHds.AddAsync(creditSaleHd);
        }

        public async Task AddDtAsync(SalCreditsaleDt creditSaleDt)
        {
            await context.SalCreditsaleDts.AddAsync(creditSaleDt);
        }

        public async Task AddLogAsync(SalCreditsaleLog creditSaleLog)
        {
            await context.SalCreditsaleLogs.AddAsync(creditSaleLog);
        }

        public bool CheckExistsRefNo(SalCreditsaleHd obj)
        {
            return context.SalCreditsaleHds.Any(x => x.DocType == "Invoice"
                           && x.CompCode == obj.CompCode
                           && x.BrnCode == obj.BrnCode
                           && x.LocCode == obj.LocCode
                           && x.RefNo == obj.RefNo);
        }
    }
}
