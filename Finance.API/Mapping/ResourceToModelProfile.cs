using AutoMapper;
using Finance.API.Domain.Models.Queries;
using Finance.API.Resources.Expense;
using Finance.API.Resources.Recive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<ReceiveHdQueryResource, ReceiveHdQuery>();
            CreateMap<ExpenseQueryResource, ExpenseQuery>();
        }
    }
}
