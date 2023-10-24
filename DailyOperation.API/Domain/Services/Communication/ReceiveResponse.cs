using DailyOperation.API.Resources.POS;

namespace DailyOperation.API.Domain.Services.Communication
{
    public class ReceiveResponse : BaseResponse<SaveReceiveResource>
    {
        public ReceiveResponse(SaveReceiveResource receive) : base(receive) { }

        public ReceiveResponse(string message) : base(message) { }
    }
}
