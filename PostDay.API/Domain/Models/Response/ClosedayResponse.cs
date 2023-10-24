using PostDay.API.Domain.Models.PostDay;
using PostDay.API.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Models.Response
{
    public class ClosedayResponse : BaseResponse<SaveDocumentRequest>
    {
        public ClosedayResponse(SaveDocumentRequest unlock) : base(unlock) { }

        public ClosedayResponse(string message) : base(message) { }
    }
}
