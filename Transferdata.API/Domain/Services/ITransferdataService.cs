using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Services.Communication;
using Transferdata.API.Resources;
using Transferdata.API.Resources.Transferdata;

namespace Transferdata.API.Domain.Services
{
    public interface ITransferdataService
    {
        Task<SalCashsaleHdResponse> SaveCashSaleAsync(IEnumerable<SalCashsaleHd> salCashsaleHds);

        Task<SalCreditsaleHdResponse> SaveCreditSaleAsync(IEnumerable<SalCreditsaleHd> salCreditsaleHds);

        Task<LogResource> UpdateCloseDayAsync(TransferDataResource query);

        Task CraateTaxInvoiceAsync(TransferDataResource query);

        Task<List<SalCashsaleHd>> ListCashSaleAsync(TransferDataResource query);

        Task<List<SalCreditsaleHd>> ListCreditSaleAsync(TransferDataResource query);

    }
}
