using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class SapMovementTypeMapping
    {
        public string SrcName { get; set; }
        public string ReasonId { get; set; }
        public string MovementType { get; set; }
        public string ControlVersion { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
