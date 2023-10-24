using MasterData.API.Domain.Services.Communication;

namespace MasterData.API.Domain.Models.DocPattern.Response
{
    public class DocPatternResponse : BaseResponse<DocPattern>
    {
        public DocPatternResponse(DocPattern docPattern) : base(docPattern) { }

        public DocPatternResponse(string message) : base(message) { }
    }
}
