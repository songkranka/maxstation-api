using System;

namespace Finance.API.Resources.Expense
{
    public class ExpenseQueryResource : QueryResource
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Keyword { get; set; }
    }
}
