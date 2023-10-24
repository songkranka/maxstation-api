using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfSapMm041Log
    {
        public string BillOfLading { get; set; }
        public string Plant { get; set; }
        public string PstngDate { get; set; }
        public string MoveType { get; set; }
        public string MovePlant { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
