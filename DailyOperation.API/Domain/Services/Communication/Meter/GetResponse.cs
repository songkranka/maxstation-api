using DailyOperation.API.Domain.Models.Meter;
using DailyOperation.API.Domain.Services.Communication;
using MaxStation.Entities.Models;
using System.Collections.Generic;

namespace DailyOperation.API.Domain.Services.Communication
{
    public class GetResponse: BaseResponse<List<GetDocumentResponse>>
    {
        public GetResponse(List<GetDocumentResponse> resource, int totalItems) : base(resource, totalItems)
        {
        }

        public GetResponse(string message) : base(message)
        {
        }
    }
}
