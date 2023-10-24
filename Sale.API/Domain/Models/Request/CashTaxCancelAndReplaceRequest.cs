using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Models.Request
{
    public class CashTaxCancelAndReplaceRequest
    {
        public _CashTax CancelCashTax { get; set; }
        public _CashTax NewCashTax { get; set; }
        public _FinBalance FinBalance { get; set; }
        public class _CashTax
        {
            public string CompCode { get; set; }
            public string BrnCode { get; set; }
            public string LocCode { get; set; }
            public string DocNo { get; set; }
            public string DocStatus { get; set; }
            public string DocType { get; set; }
            public DateTime? DocDate { get; set; }
            public string CustCode { get; set; }
            public string CitizenId { get; set; }
            public string CustName { get; set; }
            public string CustAddr1 { get; set; }
            public string CustAddr2 { get; set; }
            public string RefNo { get; set; }
            public int? ItemCount { get; set; }
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
            public string Post { get; set; }
            public string RefDocNo { get; set; }
            public int? RunNumber { get; set; }
            public string DocPattern { get; set; }
            public Guid? Guid { get; set; }
            public int? PrintCount { get; set; }
            public string PrintBy { get; set; }
            public DateTime? PrintDate { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string UpdatedBy { get; set; }
            public List<_SalTaxinvoiceDt> SalTaxinvoiceDt { get; set; }
        }

        public class _SalTaxinvoiceDt
        {
            public string CompCode { get; set; }
            public string BrnCode { get; set; }
            public string LocCode { get; set; }
            public string DocNo { get; set; }
            public int SeqNo { get; set; }
            public string LicensePlate { get; set; }
            public string PdId { get; set; }
            public string PdName { get; set; }
            public bool? IsFree { get; set; }
            public string UnitId { get; set; }
            public string UnitBarcode { get; set; }
            public string UnitName { get; set; }
            public decimal? ItemQty { get; set; }
            public decimal? StockQty { get; set; }
            public decimal? UnitPrice { get; set; }
            public decimal? UnitPriceCur { get; set; }
            public decimal? SumItemAmt { get; set; }
            public decimal? SumItemAmtCur { get; set; }
            public decimal? DiscAmt { get; set; }
            public decimal? DiscAmtCur { get; set; }
            public decimal? DiscHdAmt { get; set; }
            public decimal? DiscHdAmtCur { get; set; }
            public decimal? SubAmt { get; set; }
            public decimal? SubAmtCur { get; set; }
            public string VatType { get; set; }
            public int? VatRate { get; set; }
            public decimal? VatAmt { get; set; }
            public decimal? VatAmtCur { get; set; }
            public decimal? TaxBaseAmt { get; set; }
            public decimal? TaxBaseAmtCur { get; set; }
            public decimal? TotalAmt { get; set; }
            public decimal? TotalAmtCur { get; set; }
        }

        public class _FinBalance
        {
            public string BlNo { get; set; }
            public string CompCode { get; set; }
            public string BrnCode { get; set; }
            public string LocCode { get; set; }
            public string DocType { get; set; }
            public string DocNo { get; set; }
            public DateTime? DocDate { get; set; }
            public DateTime? DueDate { get; set; }
            public string CustCode { get; set; }
            public string Currency { get; set; }
            public decimal? NetAmt { get; set; }
            public decimal? NetAmtCur { get; set; }
            public decimal? BalanceAmt { get; set; }
            public decimal? BalanceAmtCur { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string UpdatedBy { get; set; }
        }
    }
}
