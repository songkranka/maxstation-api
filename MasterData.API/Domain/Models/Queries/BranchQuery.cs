using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Queries
{
    public class BranchQuery : Query
    {
        public string CompCode { get; set; }
        public string Keyword { get; set; }

        public BranchQuery(string compCode, string keyword, int page, int itemsPerPage) : base(page, itemsPerPage)
        {
            CompCode = compCode;
            Keyword = keyword;
        }
    }
}
