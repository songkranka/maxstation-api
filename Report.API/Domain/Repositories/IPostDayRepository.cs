using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Repositories
{
    public interface IPostDayRepository
    {        
        MemoryStream GetWorkDateExcel(PostDayRequest req);
    }
}
