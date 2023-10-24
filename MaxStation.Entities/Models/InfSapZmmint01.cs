using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfSapZmmint01
    {
        public string SrcName { get; set; }
        public string CompCode { get; set; }
        public string PostingDate { get; set; }
        public string DocumentDate { get; set; }
        public string DocumentNo { get; set; }
        public string Material { get; set; }
        public string ItemNo { get; set; }
        public string MovementType { get; set; }
        public string SaleOrg { get; set; }
        public string DistributeChannel { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }
        public string Plant { get; set; }
        public string Slog { get; set; }
        public string Costcenter { get; set; }
        public string Customer { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SalCompCode { get; set; }
        public string SalBrnCode { get; set; }
        public string SalLocCode { get; set; }
        public string SalDocNo { get; set; }
        public DateTime SalDocDate { get; set; }
        public string SalPdId { get; set; }
        public string SalPdName { get; set; }
    }
}
