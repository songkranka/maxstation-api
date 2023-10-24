using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Models
{
    public class ModelBillPayment
    {
    }

    public class GetPostDayParam
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime? DocDate { get; set; }
    }

    public class GetPostDayResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public DopPostdayHd[] Result { get; set; }
    }

    public class UpdateBillPaymentParam
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime[] ArrDocDate { get; set; }
        public bool IsUpdateBillPayment { get; set; }
    }

    public class UpdateBillPaymentResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }        

    }
}
