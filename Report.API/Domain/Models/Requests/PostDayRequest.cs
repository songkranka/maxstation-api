using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Requests
{
    public class PostDayRequest
    {
        public string CompCode { get; set; }
        public DocType Type { get; set; }
        public DateTime? DocDate { get; set; }

        public enum DocType
        {
            All = 0,
            Equal = 1,
            Lessthan = 2,
        }
    }


}
