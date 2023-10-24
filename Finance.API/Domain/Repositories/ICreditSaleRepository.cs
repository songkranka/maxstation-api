using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Domain.Repositories
{
    public interface ICreditSaleRepository
    {
        SalCreditsaleHd GetCreditSaleByTxNo(string txNo);
    }
}
