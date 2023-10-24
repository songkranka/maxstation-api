using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{
    public class PostDayResponse
    {

    }

    public class WorkDateResponse
    {
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string workDate { get; set; }
        public string updatedDate { get; set; }

    }

}
