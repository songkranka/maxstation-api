using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasCompany
    {
        public string CompCode { get; set; }
        public string MapCompCode { get; set; }
        public string CompStatus { get; set; }
        public string CompSname { get; set; }
        public string CompName { get; set; }
        public string CompNameEn { get; set; }
        public string Address { get; set; }
        public string AddressEn { get; set; }
        public string SubDistrict { get; set; }
        public string SubDistrictEn { get; set; }
        public string District { get; set; }
        public string DistrictEn { get; set; }
        public string Province { get; set; }
        public string ProvinceEn { get; set; }
        public string Postcode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string RegisterId { get; set; }
        public string CustCode { get; set; }
        public string CompImage { get; set; }
        public string IsLogin { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
