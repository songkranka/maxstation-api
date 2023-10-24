using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Repositories
{
    public interface IGovermentRepository
    {
        Gov01Response GetGov01PDF(GovermentRequest req);
        Gov03Response GetGov03PDF(GovermentRequest req);
        List<Gov05Response> GetGov05PDF(GovermentRequest req);
        Task<List<Gov06Response>> GetGov06PDF(GovermentRequest req);
        List<Gov07Response> GetGov07PDF(GovermentRequest req);
        List<Gov08Response> GetGov08PDF(GovermentRequest req);
        List<Gov09Response> GetGov09PDF(GovermentRequest req);
        List<Gov11Response> GetGov11PDF(GovermentRequest req);
    }
}
