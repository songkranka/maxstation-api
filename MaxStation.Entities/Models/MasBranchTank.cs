using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasBranchTank
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
}
