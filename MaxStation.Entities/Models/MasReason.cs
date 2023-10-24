using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasReason
    {
        public string ReasonGroup { get; set; }
        public string ReasonId { get; set; }
        public string ReasonStatus { get; set; }
        public string ReasonDesc { get; set; }
        public string IsValidate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
