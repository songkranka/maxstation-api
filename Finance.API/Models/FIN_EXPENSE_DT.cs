using System;
using System.ComponentModel.DataAnnotations;

namespace Finance.API.Models
{
    public class FinExpenseDt
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public int SeqNo { get; set; }
        public string ExpenseNo { get; set; }
        public string Parent { get; set; }
        public string CateName { get; set; }
        public string BaseName { get; set; }
        public decimal? BaseQty { get; set; }
        public string BaseUnit { get; set; }
        public string ItemName { get; set; }
        public decimal? ItemQty { get; set; }
    }
}
