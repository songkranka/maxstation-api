using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class LogStoreProcedure
    {
        public int SeqNo { get; set; }
        public string StoreProcedureName { get; set; }
        public string Status { get; set; }
        public string ErrorMsg { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
