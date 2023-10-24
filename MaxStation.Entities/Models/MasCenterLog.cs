using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasCenterLog
    {
        public Guid Id { get; set; }
        public string Method { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string Body { get; set; }
        public string Ipaddress { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
