using System;

namespace Finance.API.Models
{
    public class MasExpense
    {
        public string ExpenseNo { get;set;}
        public string ExpenseStatus { get;set;}
        public string ExpenseName { get;set;}
        public decimal? ExpenseQty { get; set; }
        public string LockQty { get; set; }
        public string ExpenseUnit { get; set; }
        public string ExpenseData { get; set; }
        public string LockData { get; set; }
        public string Parent { get; set; }
        public int? SeqNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
