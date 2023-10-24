using AutoMapper;
using MaxStation.Entities.Models;
using Report.API.Domain.Models.Response;
using Report.API.Resources.TaxInvoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<TaxInvoice, TaxInvoiceResource>()
                .ForMember(x => x.CompanyName, m => m.MapFrom(s => s.MasCompany.CompName))
                .ForMember(x => x.CompanyImage, m => m.MapFrom(s => s.MasCompany.CompImage));

            CreateMap<MasBranchDisp, DopPeriodMeter>().ReverseMap();
        }


    }
}
