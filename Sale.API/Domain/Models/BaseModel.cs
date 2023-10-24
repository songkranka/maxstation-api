using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Models
{
    public class ResponseData<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int TotalItems { get; set; }
    }

    public class ResponseService<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class RequestData
    {
        public string CompCode { get; set; }
        public string CustCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public DateTime DocDate { get; set; }
        public string Keyword { get; set; }
        public string Guid { get; set; }
        public string PDListID { get; set; }
        public string PDBarcodeList { get; set; }
        public string DocStatus { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime SysDate { get; set; }
    }
}
