using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasBranchCalibrate
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string TankId { get; set; }
        public int SeqNo { get; set; }
        public string PdId { get; set; }
        public string PdName { get; set; }
        public decimal? LevelNo { get; set; }
        public string LevelUnit { get; set; }
        public decimal? TankQty { get; set; }
        public DateTime? TankStart { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
