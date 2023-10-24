using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Resources.Employee
{
    public class EmployeeQueryResource : QueryResource
    {
        public string Keyword { get; set; }
    }
}
