using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Models.Queries
{
    public class ReceiveOilHdQuery : Query
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public string LocCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Keyword { get; set; }
        public Guid Guid { get; set; }
        public ReceiveOilHdQuery(string brnCode, string compCode, string locCode, DateTime? fromDate, DateTime? toDate, string keyword, Guid guid, int page, int itemsPerPage) : base(page, itemsPerPage)
        {
            BrnCode = brnCode;
            CompCode = compCode;
            LocCode = locCode;
            FromDate = fromDate;
            ToDate = toDate;
            Keyword = keyword;
            Guid = guid;
        }
    }
}
