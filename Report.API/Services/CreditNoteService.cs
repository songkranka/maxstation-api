using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System.Reflection.Emit;
using System;
using System.Threading.Tasks;

namespace Report.API.Services
{
    public class CreditNoteService : ICreditNoteService
    {
        private readonly ICreditNoteRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreditNoteService(
            ICreditNoteRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }


        public async Task<CreditNoteResponse.CreditNoteHd> GetDocumentAsync(CreditNoteRequest request)
        {
            return await _repository.GetDocumentAsync(request);
        }
    }
}
