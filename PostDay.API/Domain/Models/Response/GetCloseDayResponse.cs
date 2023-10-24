using PostDay.API.Domain.Models.PostDay;
using PostDay.API.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Models.Response
{
    public class GetCloseDayResponse : BaseResponse<GetDocumentResponse>
    {
        public GetCloseDayResponse(GetDocumentResponse item) : base(item) { }

        public GetCloseDayResponse(string message) : base(message) { }
    }
}
