using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Request
{
    public class SaveEmployeeAuthRequest
    {
        public string User { get; set; }
        public List<EmployeeAuth> _EmployeeAuth { get; set; }

        public class EmployeeAuth
        {
            public string EmpCode { get; set; }
            public string EmpName { get; set; }
            public int AuthCode { get; set; }
            public string PositionCode { get; set; }
            public string UpdatedBy { get; set; }
            public DateTime UpdatedDate { get; set; }

        }
    }
}
