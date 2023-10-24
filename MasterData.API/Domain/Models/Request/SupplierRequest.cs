using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Request
{
    public class ModelGetSupplierListParam
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string KeyWord { get; set; }
        public int ItemPerPage { get; set; }
        public int PageIndex { get; set; }
    }

    public class ModelGetSupplierListResult
    {
        public int TotalItem { get; set; }
        public MasSupplier[] ArrSuplier { get; set; }
    }

    public class ModelSupplier
    {
        public MasSupplier Supplier { get; set; }

        public MasSupplierProduct[] ArrSupProduct { get; set; }
    }

}
