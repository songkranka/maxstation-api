using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class TmpBranchPilot
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string MapCompCode { get; set; }
        public string Sale { get; set; }
        public string Finance { get; set; }
        public string Inventory { get; set; }
        public string Meter { get; set; }
        public DateTime? MeterDate { get; set; }
        public string Postday { get; set; }
        public DateTime? PostdayDate { get; set; }
        public string Remark { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
