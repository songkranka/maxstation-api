using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MaxStation.Entities.Models
{
    public partial class MasBranchTank
    {
        [NotMapped]
        public string PdImage { get; set; }
        [NotMapped]
        public decimal Unitprice { get; set; }
        [NotMapped]
        public decimal BeforeQty { get; set; }
        [NotMapped]
        public decimal ReceiveQty { get; set; }
        [NotMapped]
        public decimal TransferQty { get; set; }
        [NotMapped]
        public decimal IssueQty { get; set; }
        [NotMapped]
        public decimal RemainQty { get; set; }
        [NotMapped]
        public decimal WithdrawQty { get; set; }
        [NotMapped]
        public decimal SaleQty { get; set; }
        [NotMapped]
        public decimal Height { get; set; }
        [NotMapped]
        public decimal RealQty { get; set; }
        [NotMapped]
        public decimal DiffQty { get; set; }
        [NotMapped]
        public decimal WaterHeight { get; set; }
        [NotMapped]
        public decimal WaterQty { get; set; }
        [NotMapped]
        public string Hold { get; set; }
        [NotMapped]
        public string HoldReasonId { get; set; }
        [NotMapped]
        public string HoldReasonDesc { get; set; }
    }
}
