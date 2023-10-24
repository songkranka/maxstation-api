using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Models
{


    public class ModelParamSearchDelivery
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

    public class ModelResultSearchDelivery
    {
        public InvDeliveryCtrlHd[] Items { get; set; } = Array.Empty<InvDeliveryCtrlHd>();
        public int TotalItems { get; set; } = 0;
        public int ItemsPerPage { get; set; } = 0;
    }


    public class ModelParamSearchReceive
    {
        public string[] ArrPotypeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public string LocCode { get; set; }
        public string Keyword { get; set; }
    }

    public class ModelDeliveryControl
    {
        public InvDeliveryCtrlHd Header { get; set; }

        public InvDeliveryCtrlDt[] ArrDetail { get; set; }
    }
}
