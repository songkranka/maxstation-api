using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasBranchDisp
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
