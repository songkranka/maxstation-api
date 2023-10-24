using MaxStation.Entities.Models;
using PostDay.API.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Services
{
    public interface IBillPaymentService
    {
        Task<List<MasBank>> GetBankList();
        Task<GetPostDayResult> GetPostDay(GetPostDayParam param);
        Task<UpdateBillPaymentResult> UpdateBillPayment(UpdateBillPaymentParam param);
    }
}
