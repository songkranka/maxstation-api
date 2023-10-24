using DailyOperation.API.Domain.Services.Communication;
using MaxStation.Entities.Models;

namespace Inventory.API.Domain.Services.Communication
{
    public class SaveResponse: BaseResponse<DopPeriodMeter>
    {
        public SaveResponse(DopPeriodMeter resource) : base(resource)
        {
        }

        public SaveResponse(string message) : base(message)
        {
        }
    }
}
