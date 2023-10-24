using AutoMapper;
using PostDay.API.Domain.Models.Queries;
using PostDay.API.Resources.CashSale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<CashSaleHdQueryResource, CashSaleHdQuery>();
        }
    }
}
