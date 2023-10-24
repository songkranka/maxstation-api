using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses
{
    public class Products : MasProduct
    {
        public string UnitBarcode { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
