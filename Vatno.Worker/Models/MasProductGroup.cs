﻿#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasProductGroup
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
