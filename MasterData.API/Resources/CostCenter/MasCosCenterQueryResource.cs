using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Resources.CostCenter
{
    public class MasCosCenterQueryResource : QueryResource
    {
        public string CompCode { get; set; }
        public string BrnName { get; set; }
        public string BrnCode { get; set; }
        public string MapBrnCode { get; set; }
    }
}
