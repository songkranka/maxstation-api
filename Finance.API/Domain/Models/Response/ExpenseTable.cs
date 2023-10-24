using System.Collections.Generic;

namespace Finance.API.Domain.Models.Response
{
    public class ExpenseTable
    {
        public string Id { get; set; }
        public string Header { get; set; }
        public List<ExpenseTableBody> Body { get; set; }
        public bool IsExpanded { get; set; } = false;
        public class ExpenseTableBody
        {
            public string CategoryId { get; set; }
            public int? IndexListId { get; set; }
            public string Title { get; set; }
            public decimal? Qty { get; set; }
            public string DisabledQty { get; set; }
            public string LockData { get; set; }
            public string Unit { get; set; }
            public string Data { get; set; }
            public decimal? Number { get; set; }
            public bool IsDelete { get; set; } = false;
            public string Delete { get; set; }
            public int? SeqNo { get; set; }
        }
    }
}
