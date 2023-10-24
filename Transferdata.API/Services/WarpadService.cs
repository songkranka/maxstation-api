using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;
using Transferdata.API.Resources.Transferdata;

namespace Transferdata.API.Services
{
    public class WarpadService : IWarpadService
    {
        
        private readonly IWarpadRepository _warpadRepository;
        private PTMaxstationContext Context;

        public WarpadService(IWarpadRepository warpadRepository,PTMaxstationContext context)
        {
            _warpadRepository = warpadRepository;            
            Context = context;            
        }


        public async Task<WarpadCloseDay> SendCloseDay(TransferDataResource query)
        {
           return await _warpadRepository.SendCloseDay(query);
        }
    }
}
