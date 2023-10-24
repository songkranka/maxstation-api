using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Domain.Models
{
    public class ResponseData<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class RequestData
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public string Keyword { get; set; }
        public string Guid { get; set; }
        public string PDListID { get; set; }
        public string DocStatus { get; set; }
        public DateTime CloseDate { get; set; }
    }

    public class GetNotiParam
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
    }
}
