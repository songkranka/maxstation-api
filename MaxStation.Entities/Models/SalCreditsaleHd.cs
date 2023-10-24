using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class SalCreditsaleHd
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocType { get; set; }
        public string DocNo { get; set; }
        public string DocStatus { get; set; }
        public DateTime? DocDate { get; set; }
        public string Period { get; set; }
        public string QtNo { get; set; }
        public string RefNo { get; set; }
        public string CustCode { get; set; }
        public string CitizenId { get; set; }
        public string CustName { get; set; }
        public string CustAddr1 { get; set; }
        public string CustAddr2 { get; set; }
        public int? ItemCount { get; set; }
        public string Remark { get; set; }
        public string Currency { get; set; }
        public decimal? CurRate { get; set; }
        public decimal? SubAmt { get; set; }
        public decimal? SubAmtCur { get; set; }
        public string DiscRate { get; set; }
        public decimal? DiscAmt { get; set; }
        public decimal? DiscAmtCur { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? TotalAmtCur { get; set; }
        public decimal? TaxBaseAmt { get; set; }
        public decimal? TaxBaseAmtCur { get; set; }
        public int? VatRate { get; set; }
        public decimal? VatAmt { get; set; }
        public decimal? VatAmtCur { get; set; }
        public decimal? NetAmt { get; set; }
        public decimal? NetAmtCur { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string TxNo { get; set; }
        public string Post { get; set; }
        public int? RunNumber { get; set; }
        public string DocPattern { get; set; }
        public string PosNo { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
