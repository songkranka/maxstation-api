using Inventory.API.Domain.Repositories;
using Inventory.API.Helpers;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Repositories
{
    public class SupplyRequestRepository : SqlDataAccessHelper, ISupplyRequestRepository
    {
        public SupplyRequestRepository(PTMaxstationContext context) : base(context) { }

    }
}
