using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class DopPeriodEmp
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime DocDate { get; set; }
        public int PeriodNo { get; set; }
        public int SeqNo { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
    }
}
