using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{
    public class ReportAuditDiffResponse
    {
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string docNo { get; set; }
        public string docDate { get; set; }

        public decimal balanceQty { get; set; }
        public decimal itemQty { get; set; }
        public decimal diffQty { get; set; }
        public decimal adjustQty { get; set; }
        public decimal sumTotal { get; set; }
        public decimal noAdjustQty { get; set; }

        public List<Audit> audits { get; set; }

        public class Audit
        {
            public string productGroupId { get; set; }
            public string productGroupName { get; set; }
            public List<AuditDt> auditDts { get; set; }
        }

        public class AuditDt
        {
            public int seqNo { get; set; }
            public string productId { get; set; }
            public string productName { get; set; }
            public decimal productPrice { get; set; }
            public string productGroupId { get; set; }
            public string productGroupName { get; set; }
            public decimal balanceQty { get; set; }
            public decimal sumBanlanceQty { get; set; }
            public decimal itemQty { get; set; }
            public decimal sumItemQty { get; set; }
            public decimal diffQty { get; set; }
            public decimal sumDiffQty { get; set; }
            public decimal adjustQty { get; set; }
            public decimal sumAdjustQty { get; set; }
            public decimal noAdjustQty { get; set; }
            public decimal sumNoAdjustQty { get; set; }
            public decimal total { get; set; }
            public decimal sumTotal { get; set; }
        }
    }
}
