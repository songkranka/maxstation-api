using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses
{
    public class BranchTankResponse
    {
        public List<MasBranchTank> MasBranchTanks { get; set; }
        public List<MasBranchDisp> MasBranchDisps { get; set; }
    }
}
