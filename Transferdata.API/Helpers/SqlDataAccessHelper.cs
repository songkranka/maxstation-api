using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaxStation.Entities.Models;

namespace Transferdata.API.Helpers
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
