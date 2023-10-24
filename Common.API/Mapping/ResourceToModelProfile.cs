using AutoMapper;
using Common.API.Domain.Models.Queries;
using Common.API.Resource.CashSale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<CashSaleHdQueryResource, CashSaleHdQuery>();
           
        }
    }
}
