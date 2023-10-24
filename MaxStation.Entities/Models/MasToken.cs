using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasToken
    {
        public string Mid { get; set; }
        public string AccountName { get; set; }
        public string BranchName { get; set; }
        public string Email { get; set; }
        public string SapCode { get; set; }
        public string Token { get; set; }
        public string Status { get; set; }
    }
}
