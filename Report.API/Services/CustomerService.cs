using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using Report.API.Repositories;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Report.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository repo;
        public CustomerService(ICustomerRepository repository)
        {
            repo = repository;
        }

        public async Task<MemoryStream> GetDebtor02ExcelAsync(CustomerRequest req)
        {
            return await repo.GetDebtor02ExcelAsync(req);
        }

        public List<Debtor02Response> GetDebtor02PDF(CustomerRequest req)
        {
            return repo.GetDebtor02PDF(req);
        }

        public async Task<MemoryStream> ExportExcelAsync(ExportExcelRequest req)
        {
            return await repo.ExportExcelAsync(req);
        }
    }
}
