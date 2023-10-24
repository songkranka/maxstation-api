using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Services
{
    public interface IPostDayService
    {
        MemoryStream GetWorkDateExcel(PostDayRequest req);
    }
}
