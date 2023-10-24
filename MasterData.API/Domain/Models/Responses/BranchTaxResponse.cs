using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses
{
    public class BranchTaxResponse
    {
        public List<MasBranchConfig> MasBranchConfig { get; set; }
        public List<MasBranchTax> MasBranchTaxs { get; set; }
    }
}
