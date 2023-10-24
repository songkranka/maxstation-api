using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Request
{
    public class BranchRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string PdGroupID { get; set; }
        public string DocNo { get; set; }
        public string DocType { get; set; }
        public string DocumentTypeID { get; set; }
        public DateTime DocDate { get; set; }
        public string Keyword { get; set; }
        public string PDListID { get; set; }
        public string PDBarcodeList { get; set; }
    }

    public class BranchMeterRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public int PeriodNo { get; set; }
        public string PeriodStart { get; set; }
        public string DocDate { get; set; }
    }

    public class TestRequest
    {
        public string BrnCode { get; set; }
        public DateTime DocDate { get; set; }
    }

}
