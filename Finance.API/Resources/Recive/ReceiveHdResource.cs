using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Resources.Recive
{
    public class ReceiveHdResource
    {
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string CustCode { get; set; }
        public string CustName { get; set; }
        public string DocStatus { get; set; }
        public decimal? NetAmt { get; set; }
        public Guid Guid { get; set; }
    }
}
