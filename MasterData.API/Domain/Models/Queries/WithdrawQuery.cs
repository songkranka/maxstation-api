using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Queries
{
    public class WithdrawQuery : Query
    {
        public string Keyword { get; set; }
        public string PdId { get; set; }
        public WithdrawQuery(string keyword, string pdId, int page, int itemsPerPage) : base(page, itemsPerPage)
        {
            Keyword = keyword;
            PdId = pdId;
        }
    }
}
