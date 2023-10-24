using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses
{
    public class RespDocType
    {
        public MasDocPattern MasDocPattern { get; set; }
        public string DocType { get; set; }
        public string Pattern { get; set; }
    }
}
