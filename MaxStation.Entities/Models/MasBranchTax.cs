using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasBranchTax
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string TaxId { get; set; }
        public string TaxName { get; set; }
        public decimal TaxAmt { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
