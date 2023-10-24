using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Request
{
    public class MasControlRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string CtrlCode { get; set; }
    }
}
