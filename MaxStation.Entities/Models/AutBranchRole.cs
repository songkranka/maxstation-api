using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class AutBranchRole
    {
        public int AuthCode { get; set; }
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string AuthName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
