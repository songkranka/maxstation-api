using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Queries
{
    public class EmployeeQuery : Query
    {
        public string Keyword { get; set; }
        public EmployeeQuery(string keyword, int page, int itemsPerPage) : base(page, itemsPerPage)
        {
            Keyword = keyword;
        }
    }

    public class EmployeeQueryByBranch 
    { 
        public string BrnCode { get; set; }
    }
}
