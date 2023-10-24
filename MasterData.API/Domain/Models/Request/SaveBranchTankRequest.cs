using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Request
{
    public class SaveBranchTankRequest
    {
        public List<BranchTank> MasBranchTanks { get; set; }
        public List<BranchDisp> MasBranchDisps { get; set; }
        public class BranchTank
        {
            public string CompCode { get; set; }
            public string BrnCode { get; set; }
            public string TankId { get; set; }
            public string TankStatus { get; set; }
            public string PdId { get; set; }
            public string PdName { get; set; }
            public decimal? Capacity { get; set; }
            public decimal? CapacityMin { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string UpdatedBy { get; set; }
        }

        public class BranchDisp
        {
            public string CompCode { get; set; }
            public string BrnCode { get; set; }
            public string DispId { get; set; }
            public string DispStatus { get; set; }
            public int? MeterMax { get; set; }
            public string SerialNo { get; set; }
            public string TankId { get; set; }
            public string PdId { get; set; }
            public string PdName { get; set; }
            public string UnitId { get; set; }
            public string UnitBarcode { get; set; }
            public int? HoseId { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string UpdatedBy { get; set; }
        }
    }
}
