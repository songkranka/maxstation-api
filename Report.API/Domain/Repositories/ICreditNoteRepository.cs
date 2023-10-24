using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System.Threading.Tasks;

namespace Report.API.Domain.Repositories
{
    public interface ICreditNoteRepository
    {
        Task<CreditNoteResponse.CreditNoteHd> GetDocumentAsync(CreditNoteRequest request);
    }
}
