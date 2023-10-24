using MaxStation.Entities.Models;
using Sale.API.Resources;
using Sale.API.Resources.Billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Services
{
    interface IBillingService
    {
        Task<QueryResultResource<SalBillingHd>> SearchBilling(BillingQueryResource.SearchBillingQueryResource query);
    }
}
