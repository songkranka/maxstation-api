using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Queries
{
    public class ReasonQuery : Query
    {
        public string ReasonGroup { get; set; }
        public ReasonQuery(int page, int itemsPerPage) : base(page, itemsPerPage)
        {
        }
    }
}
