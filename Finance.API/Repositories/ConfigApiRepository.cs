using Finance.API.Domain.Repositories;
using Finance.API.Helpers;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Repositories
{
    public class ConfigApiRepository : SqlDataAccessHelper, IConfigApiRepository
    {
        public ConfigApiRepository(PTMaxstationContext context) : base(context)
        {

        }

        public SysConfigApi GetApiConfigBySystemIdAndApiId(string systemId, string apiId)
        {
            return context.SysConfigApis.FirstOrDefault(x => x.SystemId == systemId && x.ApiId == apiId);
        }
    }
}
