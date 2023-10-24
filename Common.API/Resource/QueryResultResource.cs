using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Resource
{
    public class QueryResultResource<T>
    {
        public int TotalItems { get; set; } = 0;
        public List<T> Items { get; set; } = new List<T>();
        public int ItemsPerPage { get; set; } = 0;
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class QueryObjectResource<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
