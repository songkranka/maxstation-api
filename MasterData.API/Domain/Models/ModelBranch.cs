using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models
{
    public class ModelBranch
    {
        public MasBranch Header { get; set; }
        public MasBranchTank[] ArrayDetailTank { get; set; }
        public MasBranchDisp[] ArrayDetailDisp { get; set; }
        public MasBranchTax[] ArrayDetailTax { get; set; }
    }
}
