using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Resources.Billing
{
    public class BillingQueryResource
    {
       public class SearchBillingQueryResource : QueryResource
        {
            public string BrnCode { get; set; }
            public string CompCode { get; set; }
            public string LocCode { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public string Keyword { get; set; }
        }        
    }

    public class ModelBillingResult
    {
        public SalBillingHd[] ArrHeader { get; set; }
        public MasEmployee[] ArrEmployee { get; set; }
        public int TotalItems { get; set; }
    }

    public class ModelBilling
    {
        public SalBillingHd Header { get; set; }
        public SalBillingDt[] ArrDetail { get; set; }
    }
}
