using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Resources;
using Transferdata.API.Resources.Transferdata;

namespace Transferdata.API.Domain.Repositories
{
    public interface ITransferdataRepository
    {
        Task AddCashsaleAsync(IEnumerable<SalCashsaleHd> salCashsaleHds);

        Task AddCreditSaleAsync(IEnumerable<SalCreditsaleHd> salCreditsaleHds);

        Task<LogResource> UpdateCloseDayAsync(TransferDataResource query);
        Task CreateTaxInvoiceAsync(TransferDataResource query);
        Task<List<SalCashsaleHd>> ListCashSaleAsync(TransferDataResource query);
        Task<List<SalCreditsaleHd>> ListCreditSaleAsync(TransferDataResource query);
    }
}
