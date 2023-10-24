using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Queries
{
    public class SysReportConfigQuery : Query
    {
        public string Group { get; set; }
        public string Keyword { get; set; }

        public SysReportConfigQuery(string group, string keyword, int page, int itemsPerPage) : base(page, itemsPerPage)
        {
            Group = group;
            Keyword = keyword;
        }
    }
}
