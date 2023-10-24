using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Resources
{
    public class QueryResultResource<T>
    {
        public int TotalItems { get; set; } = 0;
        public List<T> Items { get; set; } = new List<T>();
        public int ItemsPerPage { get; set; } = 0;
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
