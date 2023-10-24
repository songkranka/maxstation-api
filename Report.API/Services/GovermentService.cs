using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Services
{
    public class GovermentService : IGovermentService
    {
        private readonly IGovermentRepository repo;
        public GovermentService(IGovermentRepository repository)
        {
            repo = repository;
        }

        public Gov01Response GetGov01PDF(GovermentRequest req)
        {
            return repo.GetGov01PDF(req);
        }

        public Gov03Response GetGov03PDF(GovermentRequest req)
        {
            return repo.GetGov03PDF(req);
        }

        public List<Gov05Response> GetGov05PDF(GovermentRequest req)
        {
            return repo.GetGov05PDF(req);
        }

        public async Task<List<Gov06Response>> GetGov06PDF(GovermentRequest req)
        {
            return await repo.GetGov06PDF(req);
        }

        public List<Gov07Response> GetGov07PDF(GovermentRequest req)
        {
            return repo.GetGov07PDF(req);
        }

        public List<Gov08Response> GetGov08PDF(GovermentRequest req)
        {
            return  repo.GetGov08PDF(req);
        }

        public List<Gov09Response> GetGov09PDF(GovermentRequest req)
        {
            return repo.GetGov09PDF(req);
        }

        public List<Gov11Response> GetGov11PDF(GovermentRequest req)
        {
            return repo.GetGov11PDF(req);
        }
    }
}
