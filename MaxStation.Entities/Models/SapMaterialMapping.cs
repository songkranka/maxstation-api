using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class SapMaterialMapping
    {
        public string SrcName { get; set; }
        public string PdId { get; set; }
        public string SapMaterialNo { get; set; }
        public string SapUomCode { get; set; }
        public string ControlVersion { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
