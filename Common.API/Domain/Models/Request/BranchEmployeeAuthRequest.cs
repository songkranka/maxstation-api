using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Domain.Models.Request
{
    public class BranchEmployeeAuthRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string EmpCode { get; set; }
    }
}
