using AutoMapper;
using MaxStation.Entities.Models;
using PostDay.API.Domain.Models.PostDay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<CtDopPostdayHd, DopPostdayHd>().ReverseMap();
            CreateMap<Formula, DopPostdaySum>()
                .ForMember(dest => dest.SumHead, o => o.MapFrom(source => source.SourceAmount))
                .ForMember(dest => dest.SumDetail, o => o.MapFrom(source => source.DestinationAmount))
                .ReverseMap();
        }
    }
}
