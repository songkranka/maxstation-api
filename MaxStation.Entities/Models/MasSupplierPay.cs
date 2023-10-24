using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasSupplierPay
    {
        public string CompCode { get; set; }
        public string SupCode { get; set; }
        public string PayAddrId { get; set; }
        public string TaxAddrId { get; set; }
    }
}
