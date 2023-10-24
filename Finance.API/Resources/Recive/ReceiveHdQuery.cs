using Finance.API.Domain.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Resources.Recive
{
    public class ReceiveHdQuery : Query
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Keyword { get; set; }
        public Guid Guid { get; set; }

        public ReceiveHdQuery(string brnCode, string compCode, DateTime fromDate, DateTime toDate, string keyword, Guid guid, int page, int itemsPerPage) : base(page, itemsPerPage)
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
