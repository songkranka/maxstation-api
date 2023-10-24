using MasterData.API.Domain.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services.Communication
{
    public class EmployeeAuthResponse : BaseResponse<SaveEmployeeAuthRequest>
    {
        public EmployeeAuthResponse(SaveEmployeeAuthRequest employeeAuth) : base(employeeAuth) { }

        public EmployeeAuthResponse(string message) : base(message) { }
    }
}
