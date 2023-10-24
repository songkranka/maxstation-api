using System.Collections.Generic;

namespace Transferdata.API.Domain.Models
{
    public class WarpadCloseDay
    {
        public string TOPIC { get; set; }
        public string CREATE_DATE { get; set; }
        public string CREATE_TIME { get; set; }
        public string BRANCH_FROM { get; set; }
        public string LINK { get; set; }
        public List<string> DATA { get; set; }

    }
}
