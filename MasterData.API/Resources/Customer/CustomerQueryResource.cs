using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Resources.Customer
{
    public class CustomerQueryResource : QueryResource
    {
        public string Keyword { get; set; }
    }
}
