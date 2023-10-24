using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Resources.CustomerCar
{
    public class CustomerCarQueryResource : QueryResource
    {
        public string CustCode { get; set; }
        public string Keyword { get; set; }
    }
}
