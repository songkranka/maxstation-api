using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Models
{
    public class TranferPos
    {
        public List<InfPosFunction2> PosFunction2 { get; set; }
        public List<InfPosFunction4> PosFunction4 { get; set; }
        public List<InfPosFunction5> PosFunction5 { get; set; }
        public List<InfPosFunction14> PosFunction14 { get; set; }
    }
}
