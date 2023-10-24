﻿using Report.API.Domain.Models.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Report.API.Domain.Models.Requests.MeterRequest;
using static Report.API.Domain.Models.Response.MeterResponse;

namespace Report.API.Domain.Services
{
    public interface IMeterService
    {
        List<MeterRepairResponse> GetMeterRepairPDF(MeterTestResquest req);
        MemoryStream GetMeterRepairExcel(MeterTestResquest req);
        List<MeterTestResponse> GetMeterTestPDF(MeterTestResquest req);
        MemoryStream GetMeterTestExcel(MeterTestResquest req);
    }
}
