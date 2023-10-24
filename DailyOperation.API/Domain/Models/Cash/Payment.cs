using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Models.Cash
{
    public class Payment
    {
        public string JOURNAL_ID { get; set; }
        public string SITE_ID { get; set; }
        public int POS_ID { get; set; }
        public string POS_NAME { get; set; }
        public DateTime BUSINESS_DATE { get; set; }
        public int SHIFT_ID { get; set; }
        public string SHIFT_NO { get; set; }
        public string SHIFT_DESC { get; set; }
        public string MOP_INFO { get; set; }
        public string MOP_CODE { get; set; }
        public decimal AMOUNT { get; set; }
        public string BSTATUS { get; set; }
        public string INSERT_TIMESTAMP { get; set; }
        public string PONO { get; set; }
        public string CARD_NO { get; set; }
    }
}
