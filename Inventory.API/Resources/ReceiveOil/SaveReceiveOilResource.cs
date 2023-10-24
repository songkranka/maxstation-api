using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Resources.ReceiveOil
{
    public class SaveReceiveOilResource
    {
        public InvReceiveProdHd InvReceiveProdHd { get; set; }
        public List<InvReceiveProdDt> InvReceiveProdDts { get; set; }
    }
}
