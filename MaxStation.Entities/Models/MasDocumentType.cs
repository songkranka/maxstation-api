using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasDocumentType
    {
        public string DocTypeId { get; set; }
        public string DocTypeName { get; set; }
        public string DocTypeStatus { get; set; }
        public string DocTypeDesc { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
