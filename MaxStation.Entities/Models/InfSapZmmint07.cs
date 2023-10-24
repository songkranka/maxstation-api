using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfSapZmmint07
    {
        public string SrcName { get; set; }
        public string CompCode { get; set; }
        public string PostingDate { get; set; }
        public string DocumentDate { get; set; }
        public string DocumentNo { get; set; }
        public string Material { get; set; }
        public string ItemNo { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }
        public string Plant { get; set; }
        public string Slog { get; set; }
        public string Costcenter { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StcCompCode { get; set; }
        public string StcBrnCode { get; set; }
        public string StcLocCode { get; set; }
        public DateTime StcDocDate { get; set; }
        public string StcPdId { get; set; }
        public string StcPdName { get; set; }
    }
}
