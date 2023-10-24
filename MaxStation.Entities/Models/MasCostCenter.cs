using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasCostCenter
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string MapBrnCode { get; set; }
        public string BrnStatus { get; set; }
        public string BrnName { get; set; }
        public string CostCenter { get; set; }
        public string ProfitCenter { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
