using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasBillerInfo
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Business { get; set; }
        public string BankAccount { get; set; }
        public string BillerId { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string Type { get; set; }
        public string Remark { get; set; }
        public string SecretKey { get; set; }
        public string ApiKey { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
