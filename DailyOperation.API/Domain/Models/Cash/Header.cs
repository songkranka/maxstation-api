using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Models.Cash
{
    public class Header
    {
        public string SITE_ID { get; set; }
        public string JOURNAL_ID { get; set; }
        public DateTime JOURNAL_DATE { get; set; }
        public string JOURNAL_STATUS { get; set; }
        public DateTime BUSINESS_DATE { get; set; }
        public int POS_ID { get; set; }
        public string POS_NAME { get; set; }
        public string POS_REGISTERNO { get; set; }
        public string USER_ID { get; set; }
        public string USERNAME { get; set; }
        public int SHIFT_ID { get; set; }
        public string SHIFT_NO { get; set; }
        public string SHIFT_DESC { get; set; }
        public int TRANS_NO { get; set; }
        public string BILLNO { get; set; }
        public string TAXINVNO { get; set; }
        public string MAX_CARD_NUMBER { get; set; }
        public int TOTAL_GOODSAMT { get; set; }
        public decimal TOTAL_DISCAMT { get; set; }
        public decimal TOTAL_TAXAMT { get; set; }
        public decimal TOTAL_PAID_AMT { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string LIC_NO { get; set; }
        public string MILES { get; set; }
        public int REQ_DESCRIP_ID { get; set; }
        public string REQ_DESCRIP_CAT { get; set; }
        public string REQ_DESCRIP { get; set; }
        public int REASON_ID { get; set; }
    }
}
