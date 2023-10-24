using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InvDeliveryCtrlDt
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public int SeqNo { get; set; }
        public string PdId { get; set; }
        public string PdName { get; set; }
        public string CtrlFull { get; set; }
        public int? CtrlFullMm { get; set; }
        public int? CtrlFullLt { get; set; }
        public string CtrlFullContact { get; set; }
        public string CtrlApi { get; set; }
        public string CtrlApiDesc { get; set; }
        public decimal? CtrlApiStart { get; set; }
        public decimal? CtrlApiFinish { get; set; }
        public string CtrlEthanol { get; set; }
        public decimal? CtrlEthanolQty { get; set; }
    }
}
