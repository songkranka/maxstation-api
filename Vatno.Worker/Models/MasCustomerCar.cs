﻿#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasCustomerCar
    {
        public string CustCode { get; set; }
        public string LicensePlate { get; set; }
        public string CarStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
