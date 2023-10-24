using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class DopValidate
    {
        public int ValidNo { get; set; }
        public string ValidStatus { get; set; }
        public string ValidName { get; set; }
        public string Remark { get; set; }
        public string SourceName { get; set; }
        public string SourceType { get; set; }
        public string SourceKey { get; set; }
        public string SourceValue { get; set; }
        public string ValidSql { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
