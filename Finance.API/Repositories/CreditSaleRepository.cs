using Finance.API.Domain.Repositories;
using Finance.API.Helpers;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Repositories
{
    public class CreditSaleRepository : SqlDataAccessHelper, ICreditSaleRepository
    {
        public CreditSaleRepository(PTMaxstationContext context) : base(context)
        {

        }

        public SalCreditsaleHd GetCreditSaleByTxNo(string txNo)
        {
            return context.SalCreditsaleHds.FirstOrDefault(x => x.TxNo == txNo);
        }
    }
}
