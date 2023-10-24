using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Models.Queries
{
    public class QuotationHdQuery : Query
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public string Keyword { get; set; }
        public string Guid { get; set; }
        public string PDListID { get; set; }
        public string DocStatus { get; set; }
        public string CustCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public QuotationHdQuery(string comCode, string brnCode, string locCode, string docNo, string keyWord, string guId, string pdListId, string docStatus, int page, int itemsPerPage) : base(page, itemsPerPage)
        {
            CompCode = comCode;
            BrnCode = brnCode;
            LocCode = locCode;
            DocNo = docNo;
            Keyword = keyWord;
            Guid = guId;
            PDListID = pdListId;
            DocStatus = docStatus;
        }
    }
}
