#nullable disable

namespace Vatno.Worker.Models
{
    public partial class SysReportConfig
    {
        public string ReportGroup { get; set; }
        public int SeqNo { get; set; }
        public string ReportName { get; set; }
        public string ReportStatus { get; set; }
        public string ReportUrl { get; set; }
        public string ExcelUrl { get; set; }
        public string ParameterType { get; set; }
        public string IsPdf { get; set; }
        public string IsExcel { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
