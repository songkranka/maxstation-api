using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaxStation.Entities.Models;

namespace MasterData.API.Domain.Models.Responses
{
    public class MeterResponse
    {
        public string PeriodStart { get; set; }
        public string PeriodFinish { get; set; }
        public List<MasBranchDisp> MasBranchDispItems { get; set; }
        public List<MasBranchTank> MasBranchTankItems { get; set; }
        public List<DopPeriodGl> MasBranchCashDrItems { get; set; }
        public List<DopPeriodGl> MasBranchCashCrItems { get; set; }

    }
}
