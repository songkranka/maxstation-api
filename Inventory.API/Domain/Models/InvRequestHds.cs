using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Models
{
    public class InvRequestHds : InvRequestHd
    {
        public string PdGroupName { get; set; }
    }
}
