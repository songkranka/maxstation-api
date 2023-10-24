using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Models.Queries
{
    public class CashQuery : Query
    {
        public DateTime FromDate { get; set; }
        public CashQuery(DateTime fromDate, int page, int itemsPerPage) : base(page, itemsPerPage)
        {
            FromDate = fromDate;
        }
    }
}
