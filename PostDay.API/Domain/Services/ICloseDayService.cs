using PostDay.API.Domain.Models.PostDay;
using PostDay.API.Domain.Models.Response;
using PostDay.API.Domain.Services.Communication.PostDay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Services
{
    public interface ICloseDayService
    {
        Task<ClosedayResponse> SaveDocument(SaveDocumentRequest req);
        Task<GetCloseDayResponse> GetDocument(GetDocumentRequest req);
    }
}
