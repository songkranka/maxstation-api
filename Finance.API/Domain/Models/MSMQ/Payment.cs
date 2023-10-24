using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Domain.Models.MSMQ
{
    public class Payment
    {
        public string brnNew { get; set; }
        public decimal discountAmt { get; set; }
        public decimal discountPercent { get; set; }
        public string discountType { get; set; }
        public string inNo { get; set; }
        public string paymentDate { get; set; }

        public List<PaymentDetail> paymentDetailList { get; set; }
        public decimal? totalPaymentAmt { get; set; }
        public string transactionID { get; set; }
        public string transactionType { get; set; }
        public decimal? vatAmt { get; set; }
        public class PaymentDetail
        {
            public decimal debitCreditAmt { get; set; }
            public string debitCreditType { get; set; }
            public decimal debitCreditVatAmt { get; set; }
            public decimal? discountAmt { get; set; }
            public decimal discountPercent { get; set; }
            public string discountType { get; set; }
            public string itemCode { get; set; }
            public string itemName { get; set; }
            public int seqNo { get; set; }
            public decimal? totalPaymentAmt { get; set; }
            public int totalUnit { get; set; }
            public string transactionID { get; set; }
            public int unitPrice { get; set; }
            public decimal? vatAmt { get; set; }
        }
    }
}
