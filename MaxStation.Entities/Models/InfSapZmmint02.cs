using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfSapZmmint02
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
        public string TargetPlant { get; set; }
        public string RefDocTfOut { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TfoCompCode { get; set; }
        public string TfoBrnCode { get; set; }
        public string TfoLocCode { get; set; }
        public string TfoDocNo { get; set; }
        public DateTime TfoDocDate { get; set; }
        public string TfoBrnCodeTo { get; set; }
        public string TfoPdId { get; set; }
        public string TfoPdName { get; set; }
    }
}
