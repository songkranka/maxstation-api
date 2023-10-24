using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfPoHeader
    {
        public string PoNumber { get; set; }
        public string CompCode { get; set; }
        public string DocType { get; set; }
        public string DeleteInd { get; set; }
        public string Status { get; set; }
        public DateTime? CreatDate { get; set; }
        public string CreatedBy { get; set; }
        public string Vendor { get; set; }
        public string Pmnttrms { get; set; }
        public string PurchOrg { get; set; }
        public string PurGroup { get; set; }
        public string Currency { get; set; }
        public DateTime? DocDate { get; set; }
        public string DownpayType { get; set; }
        public string RetentionType { get; set; }
        public decimal? RetentionPercentage { get; set; }
        public decimal? DownpayAmount { get; set; }
        public decimal? DownpayPercent { get; set; }
        public DateTime? DownpayDuedate { get; set; }
        public string IsDeleted { get; set; }
        public DateTime? CreatedDate1 { get; set; }
        public string CreatedBy1 { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string InfStatus { get; set; }
        public string ErrorMsg { get; set; }
        public string ReceiveStatus { get; set; }
        public DateTime? ReceiveUpdate { get; set; }
        public string ReceiveBrnCode { get; set; }
        public string ReceiveLocCode { get; set; }
        public string ReceiveDocNo { get; set; }
    }
}
