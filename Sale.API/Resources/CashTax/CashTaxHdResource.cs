using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaxStation.Entities.Models;
using Sale.API.Domain.Models;
namespace Sale.API.Resources.CashTax
{
    public class CashTaxHdResource
    {
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string DocType { get; set; }
        public string CustCode { get; set; }
        public string CustName { get; set; }
        public decimal? VatAmt { get; set; }
        public decimal? VatAmtCur { get; set; }
        public string DocStatus { get; set; }
        public Guid? Guid { get; set; }
        public decimal? NetAmt { get; set; }
        public decimal? NetAmtCur { get; set; }

    }

    //public class CashTaxCancelAndReplaceResource
    //{
    //    public Sale.API.Domain.Models.CashTax CancelCashTax { get; set; }
    //    public Sale.API.Domain.Models.CashTax NewCashTax { get; set; }
    //    public FinBalance FinBalance { get; set; }
    //}
}
