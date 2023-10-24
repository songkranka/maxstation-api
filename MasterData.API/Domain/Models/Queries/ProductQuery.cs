using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Queries
{
    public class ProductQuery : Query
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string Keyword { get; set; }

        public ProductQuery(
            string compCode,
            string brnCode,
            string locCode,
            string keyword,
            int page,
            int itemsPerPage) : base(page, itemsPerPage)
        {
            CompCode = compCode;
            BrnCode = brnCode;
            LocCode = locCode;
            Keyword = keyword;
        }
    }
}
