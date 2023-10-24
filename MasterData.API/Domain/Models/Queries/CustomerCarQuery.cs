using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Queries
{
    public class CustomerCarQuery : Query
    {
        public string CustCode { get; set; }
        public string Keyword { get; set; }

        public CustomerCarQuery(string custCode, string keyword, int page, int itemsPerPage) : base(page, itemsPerPage)
        {
            CustCode = custCode;
            Keyword = keyword;
        }
    }
}
