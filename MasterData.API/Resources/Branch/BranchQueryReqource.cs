using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Resources.Branch
{
    public class BranchQueryReqource  : QueryResource
    {
        public string CompCode { get; set; }
        public string Keyword { get; set; }
    }
}
