using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Controllers.Audit
{
    public class ModelAudit
    {
        public InvAuditHd Header { get; set; }
        public InvAuditDt[] ArrayDetail { get; set; }
    }

    public class ModelAuditResult
    {
        public InvAuditHd[] ArrayHeader { get; set; }
        public MasEmployee[] ArrayEmployee { get; set; }
        public int TotalItems { get; set; }
    }

    public class ModelAuditParam
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public string LocCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Keyword { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }

    }

    public class ModelAuditProductParam
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public DateTime? StockDate { get; set; }
    }

    public class ModelAuditProduct
    {
        public MasProduct[] ArrayProduct { get; set; }

        public InvStockDaily[] ArrayStockDaily { get; set; }
    }
}
