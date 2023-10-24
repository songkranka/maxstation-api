using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Request
{
    public class EmployeeBranchConfigRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string DocDate { get; set; }
        public string EmpCode { get; set; }
    }
}
