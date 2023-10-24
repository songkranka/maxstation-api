using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Resources.SysReportConfig
{
    public class SysReportConfigQueryResource : QueryResource
    {
        public string Group { get; set; }
        public string Keyword { get; set; }
    }
}
