using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Models
{
    public class RequestWarpadModel
    {
        public string TOPIC { get; set; }
        public string CREATE_DATE { get; set; }
        public string CREATE_TIME { get; set; }
        public string BRANCH_FROM { get; set; }
        public string BRANCH_TO { get; set; }
        public string DOC_NUMBER { get; set; }
        public string LINK { get; set; }
        public List<RequestWarpadDataMedel> DATA { get; set; }
    }

    public class RequestWarpadDataMedel 
    {
        public string ITEM { get; set; }
    }
}
