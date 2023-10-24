using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Resources.Reason
{
    public class WithdrawQueryResource : QueryResource
    {
        public string Keyword { get; set; }
        public string PdId { get; set; }
    }
}
