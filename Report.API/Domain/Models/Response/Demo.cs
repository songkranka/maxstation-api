using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{
    public class Demo
    {
        public Header header { get; set; }
        public List<Detail> details { get; set; }

        public class Header
        {
            public string HeaderId { get; set; }
        }
        public class Detail
        {
            public string DetailId { get; set; }
        }
    }
}
