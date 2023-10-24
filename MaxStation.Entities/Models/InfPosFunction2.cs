using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfPosFunction2
    {
        public string SiteId { get; set; }
        public DateTime BusinessDate { get; set; }
        public string ShiftNo { get; set; }
        public int HoseId { get; set; }
        public int? PosId { get; set; }
        public string PosName { get; set; }
        public int? ShiftId { get; set; }
        public string ShiftDesc { get; set; }
        public int? PumpId { get; set; }
        public string GradeId { get; set; }
        public string GradeName { get; set; }
        public string GradeName2 { get; set; }
        public decimal? OpenMeterValue { get; set; }
        public decimal? CloseMeterValue { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? OpenMeterVolume { get; set; }
        public decimal? CloseMeterVolume { get; set; }
        public decimal? TotalVolume { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
