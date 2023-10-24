using AutoMapper;
using MaxStation.Entities.Models;
using Report.API.Domain.Models.Queries;
using Report.API.Resources.ReportSummaryOilBalance;
using Report.API.Resources.SysReportConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<TankModel, DopPeriodTank>().ReverseMap();
            CreateMap<DtTankModelForPDF, DopPeriodTank>().ReverseMap();
            CreateMap<SysReportConfigQueryResource, SysReportConfigQuery>();
        }
    }
}
