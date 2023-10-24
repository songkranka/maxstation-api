using PostDay.API.Domain.Models.PostDay;

namespace PostDay.API.Domain.Services.Communication.PostDay
{
    public class PostDayResponse : BaseResponse<GetDocumentResponse>
    {
        public PostDayResponse(GetDocumentResponse item) : base(item) { }

        public PostDayResponse(string message) : base(message) { }
    }
}
