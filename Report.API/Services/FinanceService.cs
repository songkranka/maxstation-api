using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Report.API.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly IFinanceRepository repo;
        public FinanceService(IFinanceRepository repository)
        {
            repo = repository;
        }
        public async Task<MemoryStream> GetFin03ExcelAsync(FinanceRequest req)
        {
            return await repo.GetFin03ExcelAsync(req);
        }

        public List<Fin03Response> GetFin03PDF(FinanceRequest req)
        {
            return repo.GetFin03PDF(req);
        }

        public async Task<MemoryStream> GetFin08ExcelAsync(FinanceRequest req)
        {
            return await repo.GetFin08ExcelAsync(req);
        }

        public List<Fin08Response> GetFin08PDF(FinanceRequest req)
        {
            return repo.GetFin08PDF(req);
        }
    }
}
