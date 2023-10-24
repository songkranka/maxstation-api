using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfSapZmmint04
    {
        public string SrcName { get; set; }
        public string CompCode { get; set; }
        public string PostingDate { get; set; }
        public string DocumentDate { get; set; }
        public string DocumentNo { get; set; }
        public string Material { get; set; }
        public string ItemNo { get; set; }
        public string MovementType { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }
        public string Plant { get; set; }
        public string Slog { get; set; }
        public string Costcenter { get; set; }
        public decimal? Amount { get; set; }
        public string Vendor { get; set; }
        public DateTime CreatedDate { get; set; }
        public string InxCompCode { get; set; }
        public string InxBrnCode { get; set; }
        public string InxLocCode { get; set; }
        public string InxDocNo { get; set; }
        public DateTime InxDocDate { get; set; }
        public string InxPdId { get; set; }
        public string InxPdName { get; set; }
    }
}
