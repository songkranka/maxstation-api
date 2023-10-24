using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfSapMm042Log
    {
        public string BillOfLading { get; set; }
        public string Plant { get; set; }
        public string PstngDate { get; set; }
        public string MoveType { get; set; }
        public string RefDocNo { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
