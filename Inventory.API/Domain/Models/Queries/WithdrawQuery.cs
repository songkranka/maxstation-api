using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Models.Queries
{
    public class WithdrawQuery : Query
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Keyword { get; set; }

        public WithdrawQuery( int page, int itemsPerPage) : base(page, itemsPerPage)
        {
 
        }

        //public WithdrawQuery(string brnCode, string compCode, DateTime? fromDate, DateTime? toDate, string keyword, int page, int itemsPerPage) : base(page, itemsPerPage)
        //{
        //    CompCode = compCode;
        //    BrnCode = brnCode;
        //    FromDate = fromDate;
        //    ToDate = toDate;
        //    Keyword = keyword;
        //}

    }
}
