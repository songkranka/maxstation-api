using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class BaseRepositories
    {
        protected PTMaxstationContext Context;
        public BaseRepositories(PTMaxstationContext context)
        {
            this.Context = context;
        }
    }
}
