using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasPayType
    {
        public int PayTypeId { get; set; }
        public string PayTypeStatus { get; set; }
        public string PayTypeName { get; set; }
        public string PayTypeRemark { get; set; }
        public int? PayGroupId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
