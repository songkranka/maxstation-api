using System;

namespace Finance.API.Domain.Models.Queries
{
    public class ExpenseQuery : Query
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Keyword { get; set; }

        public ExpenseQuery(int page, int itemsPerPage) : base(page, itemsPerPage)
        {

        }
    }
}
