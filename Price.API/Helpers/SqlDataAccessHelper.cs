using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Price.API.Helpers
{
    public class SqlDataAccessHelper : DataModelHelper
    {
        protected PTMaxstationContext context;

        public SqlDataAccessHelper(PTMaxstationContext context)
        {
            this.context = context;
        }
    }
}
