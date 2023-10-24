using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transferdata.API.Resources.Pos
{
    public class TranferPosQueryResource
    {
        public string BrnCode { get; set; }
        public DateTime DocDate { get; set; }
    }
}
