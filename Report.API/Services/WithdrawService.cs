using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Report.API.Domain.Models.Requests.WithdrawRequest;
using static Report.API.Domain.Models.Response.WithdrawResponse;

namespace Report.API.Services
{
    public class WithdrawService : IWithdrawService
    {
        private readonly IWithdrawRepository _repository;

        public WithdrawService(IWithdrawRepository repo)
        {
            _repository = repo;
        }


        public WithdrawHd GetDocumentPDF(GetDocumentRequest req)
        {
            return _repository.GetDocumentPDF(req);
        }
    }
}
