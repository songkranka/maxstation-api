using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using Report.API.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Report.API.Domain.Models.Requests.MeterRequest;
using static Report.API.Domain.Models.Response.MeterResponse;

namespace Report.API.Services
{
    public class MeterService : IMeterService
    {
        private readonly IMeterRepository repo;

        public MeterService(IMeterRepository meterRepository)
        {
            repo = meterRepository;
        }

        public List<MeterTestResponse> GetMeterTestPDF(MeterTestResquest req)
        {
            return repo.GetMeterTestPDF(req);
        }

        public MemoryStream GetMeterTestExcel(MeterTestResquest req)
        {
            return repo.GetMeterTestExcel(req);
        }

        public List<MeterRepairResponse> GetMeterRepairPDF(MeterTestResquest req)
        {
            return repo.GetMeterRepairPDF(req);
        }

        public MemoryStream GetMeterRepairExcel(MeterTestResquest req)
        {
            return repo.GetMeterRepairExcel(req);
        }
    }
}
