using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfSapZmmint04Log
    {
        public string DocumentNo { get; set; }
        public string CompCode { get; set; }
        public string Plant { get; set; }
        public string PostingDate { get; set; }
        public string MovementType { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
