using AutoMapper;
using Finance.API.Domain.Models.Queries;
using Finance.API.Resources;
using Finance.API.Resources.Recive;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<QueryResult<FinReceiveHd>, QueryResultResource<ReceiveHdResource>>();
        }
    }
}
