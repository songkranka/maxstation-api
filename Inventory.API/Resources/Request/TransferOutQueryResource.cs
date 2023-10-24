using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Resources.Request
{
    public class TransferOutQueryResource
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

    public class GetRequestHdListQueryResource
    {
        public string Keyword { get; set; }
        public string DocTypeId { get; set; }
        public string CompCode { get; set; }
        public string BrnCodeFrom { get; set; }
        public string DocStatus { get; set; }
        public string DocNo { get; set; }
        public DateTime? SysDate { get; set; }

    }

    public class GetRequestDtListQueryResource
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocTypeId { get; set; }
        public string DocNo { get; set; }
    }
}
