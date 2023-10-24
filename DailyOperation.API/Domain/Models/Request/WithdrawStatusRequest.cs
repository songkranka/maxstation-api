using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Models.Request
{
    public class WithdrawStatusRequest
    {
        public string DocType { get; set; }
    }

    public class GetDopPosConfigParam
    {
        public string[] ArrDocType { get; set; }
        public string[] ArrDocDesc { get; set; }
    }
}
