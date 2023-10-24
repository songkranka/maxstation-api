using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Resources
{
    public class QueryResource
    {
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
