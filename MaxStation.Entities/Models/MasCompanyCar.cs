using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasCompanyCar
    {
        public string CompCode { get; set; }
        public string LicensePlate { get; set; }
        public string CarStatus { get; set; }
        public string CarRemark { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
