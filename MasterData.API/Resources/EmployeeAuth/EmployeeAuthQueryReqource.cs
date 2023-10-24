using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Resources.EmployeeAuth
{
    public class EmployeeAuthQueryReqource : QueryResource
    {
        public string Keyword { get; set; }
    }
}
