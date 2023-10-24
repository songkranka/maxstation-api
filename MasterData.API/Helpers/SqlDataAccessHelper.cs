using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MaxStation.Entities.Models;

namespace MasterData.API.Helpers
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
