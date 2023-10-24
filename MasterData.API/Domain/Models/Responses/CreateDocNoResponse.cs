using MasterData.API.Domain.Services.Communication;

namespace MasterData.API.Domain.Models.Responses
{
    public class CreateDocNoResponse : BaseResponse<DocPattern>
    {
        public CreateDocNoResponse(DocPattern docPattern) : base(docPattern) { }

        public CreateDocNoResponse(string message) : base(message) { }
    }
}
