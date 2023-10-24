using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Models
{
    public class InvRequest : InvRequestHd
    {
        public List<InvRequestDt> InvRequestDt { get; set; }
    }
}
