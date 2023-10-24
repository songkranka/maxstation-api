using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Models.Queries
{
    public class CashTaxHdQuery : Query
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Keyword { get; set; }
        public Guid Guid { get; set; }
        public CashTaxHdQuery(string brnCode, string compCode, DateTime? fromDate, DateTime? toDate, string keyword, Guid guid, int page, int itemsPerPage) : base(page, itemsPerPage)
        {
            BrnCode = brnCode;
            CompCode = compCode;
            FromDate = fromDate;
            ToDate = toDate;
            Keyword = keyword;
            Guid = guid;
        }
    }
}
