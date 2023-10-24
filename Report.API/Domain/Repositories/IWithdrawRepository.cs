using Report.API.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Report.API.Domain.Models.Requests.WithdrawRequest;
using static Report.API.Domain.Models.Response.WithdrawResponse;

namespace Report.API.Domain.Repositories
{
    public  interface IWithdrawRepository
    {
        WithdrawHd GetDocumentPDF(GetDocumentRequest req);
    }
}
