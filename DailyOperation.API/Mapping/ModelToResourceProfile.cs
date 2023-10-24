using AutoMapper;
using DailyOperation.API.Domain.Models.Meter;
using DailyOperation.API.Domain.Models.Queries;
using DailyOperation.API.Resources;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<MasBranchDisp, DopPeriodMeter>().ReverseMap();
            CreateMap<MasBranchTank, DopPeriodTank>().ReverseMap();
            CreateMap<DopPeriodGl, DopPeriodCashGl>().ReverseMap();
            CreateMap<QueryResult<MasBranchMeterResponse>, QueryResultResource<MasBranchMeterResponse>>();
            CreateMap<QueryResult<MasBranchCalibrate>, QueryResultResource<MasBranchCalibrate>>();
        }
    }
}
