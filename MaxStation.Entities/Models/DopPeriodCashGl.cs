using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class DopPeriodCashGl
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime DocDate { get; set; }
        public int PeriodNo { get; set; }
        public int SeqNo { get; set; }
        public string GlType { get; set; }
        public string GlNo { get; set; }
        public string GlDesc { get; set; }
        public decimal? GlAmt { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
