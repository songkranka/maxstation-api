using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfSapZmmint03
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
        public string RefDocTfIn { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TfiCompCode { get; set; }
        public string TfiBrnCode { get; set; }
        public string TfiLocCode { get; set; }
        public string TfiDocNo { get; set; }
        public DateTime TfiDocDate { get; set; }
        public string TfiPdId { get; set; }
        public string TfiPdName { get; set; }
    }
}
