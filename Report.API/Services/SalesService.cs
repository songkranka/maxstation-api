using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Report.API.Services
{
    public class SalesService : ISalesService
    {
        private readonly ISalesRepository repository;

        public SalesService(ISalesRepository repo)
        {
            repository = repo;
        }

        public async Task<MemoryStream> GetSale03ExcelAsync(SalesRequest req)
        {
            return await repository.GetSale03ExcelAsync(req);
        }

        public List<Sale03Response> GetSale03PDF(SalesRequest req)
        {
            return repository.GetSale03PDF(req);
        }

        public List<Sale04Response> GetSale04PDF(SalesRequest req)
        {
            return repository.GetSale04PDF(req);
        }

        public async Task<MemoryStream> GetSale04ExcelAsync(SalesRequest req)
        {
            return await repository.GetSale04ExcelAsync(req);
        }

        public List<Sale06Response> GetSale06PDF(SalesRequest req)
        {
            return repository.GetSale06PDF(req);
        }

        public async Task<MemoryStream> GetSale06ExcelAsync(SalesRequest req)
        {
            return await repository.GetSale06ExcelAsync(req);
        }

        public async Task<List<Sale02Response>> GetSale02PDF(SalesRequest req)
        {
            return await repository.GetSale02PDF(req);
        }

        public async Task<MemoryStream> GetSale02ExcelAsync(SalesRequest req)
            => await repository.GetSale02ExcelAsync(req);
    }
}