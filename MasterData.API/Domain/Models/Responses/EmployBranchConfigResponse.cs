using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses
{
    public class EmployBranchConfigResponse
    {
        public string PositionCode { get; set; }
        public int ItemNo { get; set; }
        public string ConfigId { get; set; }
        public string IsView { get; set; }
        public string ConfigName { get; set; }
        public string IsLockDate { get; set; }
        public string IsLock { get; set; }
        public string Remark { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
