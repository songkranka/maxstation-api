using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasBranchPeriod
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public int PeriodNo { get; set; }
        public string TimeStart { get; set; }
        public string TimeFinish { get; set; }
    }
}
