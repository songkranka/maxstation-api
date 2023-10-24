using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfPosFunction14
    {
        public string SiteId { get; set; }
        public DateTime BusinessDate { get; set; }
        public string ShiftNo { get; set; }
        public string JournalId { get; set; }
        public string MopCode { get; set; }
        public int ItemNo { get; set; }
        public int? PosId { get; set; }
        public string PosName { get; set; }
        public int? ShiftId { get; set; }
        public string ShiftDesc { get; set; }
        public string MopInfo { get; set; }
        public decimal? Amount { get; set; }
        public string Bstatus { get; set; }
        public string InsertTimestamp { get; set; }
        public string Pono { get; set; }
        public string CardNo { get; set; }
        public string BranchAt { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
