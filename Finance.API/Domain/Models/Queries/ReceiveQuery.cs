using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Models.Queries
{
    public class ReceiveQuery 
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Keyword { get; set; }
        public string DocType { get; set; }
        public string DocTypeFilter { get; set; }
        public string CustCode { get; set; }
    }
}
