using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{

    public class Sale02Response
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }

        public string docDate { get; set; }
        public string acctCode { get; set; }
        public string acctName { get; set; }
        public decimal itemQty { get; set; }
        public decimal saleAmt { get; set; }
        public decimal vatAmt { get; set; }
        public decimal subAmt { get; set; }
    }


    public class Sale03Response
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string docType { get; set; }
        public string docDate { get; set; }
        public string docNo { get; set; }
        public string docStatus { get; set; }
        public string refDocNo { get; set; }
        public string remark { get; set; }
        public string custCode { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public decimal itemQty { get; set; }
        public decimal unitPrice { get; set; }
        public decimal sumItemAmt { get; set; }
        public decimal discAmt { get; set; }
        public decimal vatAmt { get; set; }
        public decimal taxBaseAmt { get; set; }
        public decimal subAmt { get; set; }


    }

    public class Sale04Response
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string docDate { get; set; }
        public string docNo { get; set; }
        public string docStatus { get; set; }
        public string custCode { get; set; }
        public string custName { get; set; }
        public string poNo { get; set; }
        public string licensePlate { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public decimal itemQty { get; set; }
        public decimal sumItemAmt { get; set; }
        public decimal discAmt { get; set; }
        public decimal subAmt { get; set; }
        public decimal vatAmt { get; set; }

    }

    public class Sale06Response
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }

        public string custCode { get; set; }
        public string custName { get; set; }

        public decimal itemQty { get; set; }
        public decimal sumItemAmt { get; set; }
        public decimal discAmt { get; set; }
        public decimal subAmt { get; set; }

    }

}
