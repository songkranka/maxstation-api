using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Queries
{
    public class EmployeeAuthQuery : Query
    {
        public string Keyword { get; set; }

        public EmployeeAuthQuery(string keyword, int page, int itemsPerPage) : base(page, itemsPerPage)
        {
            Keyword = keyword;
        }
    }
}
