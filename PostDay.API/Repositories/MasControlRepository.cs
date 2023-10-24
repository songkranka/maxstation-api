using AutoMapper;
using log4net;
using MaxStation.Entities.Models;
using PostDay.API.Domain.Repositories;
using PostDay.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Repositories
{
    public class MasControlRepository : SqlDataAccessHelper, IMasControlRepository
    {
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(CloseDayRepository));

        public MasControlRepository(
            PTMaxstationContext context,
            IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task UpdateWDateAsync(string compCode, string brnCode, string locCode, string ctrlCode, string ctrlValue)
        {

        }
    }
}
