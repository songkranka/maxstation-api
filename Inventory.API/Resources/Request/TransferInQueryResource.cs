using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Resources.Request
{
    public class TransferInQueryResource
    {

    }

    public class SearchTranInQueryResource
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string Guid { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Keyword { get; set; }
    }

    public class GetTransOutHdListQueryResource
    {
        public string Keyword { get; set; }
        //public string DocTypeId { get; set; }
        public string CompCode { get; set; }
        public string BrnCodeTo { get; set; }
        public string SysDate { get; set; }

        //public string DocStatus { get; set; }
        //public string DocNo { get; set; }

    }
}
