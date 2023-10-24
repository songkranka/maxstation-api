using Report.API.Domain.Models.Response;
using System.Collections.Generic;
using static Report.API.Domain.Models.Requests.WithdrawRequest;
using static Report.API.Domain.Models.Response.WithdrawResponse;

namespace Report.API.Domain.Services
{
    public interface IWithdrawService
    {
        WithdrawHd GetDocumentPDF(GetDocumentRequest req);
    }
}
