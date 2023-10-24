using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses
{
    public class SysBranchConfigResponse
    {
        public string BrnCode { get; set; }
        public string BrnName { get; set; }
        public DateTime DocDate { get; set; }
        public string EmpName { get; set; }
        public int SeqNo { get; set; }
        public DateTime? LockDate { get; set; }
    }
}
