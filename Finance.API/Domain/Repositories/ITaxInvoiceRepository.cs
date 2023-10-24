﻿using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Domain.Repositories
{
    public interface ITaxInvoiceRepository
    {
        List<SalTaxinvoiceDt> GetTaxInvoiceByDocNo(string docNo);
        SalTaxinvoiceHd GetTaxInvoiceHdByFinReceivePay(FinReceivePay finReceivePay, string custCode);
    }
}
