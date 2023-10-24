using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Queries
{
    public class CustomerQuery // : Query
    {
        public string Keyword { get; set; }
        public string ParentName { get; set; }
        public int Page { get;  set; }
        public int ItemsPerPage { get;  set; }
        //public CustomerQuery()
        //{

        //}
        //public CustomerQuery(string keyword, int page, int itemsPerPage) : base(page, itemsPerPage)
        //{
        //    Keyword = keyword;
        //}
    }
}
