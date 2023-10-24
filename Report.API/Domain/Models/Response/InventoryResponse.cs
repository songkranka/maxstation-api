using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{
    public class InventoryResponse
    {
    }


    public class InvWithdrawResponse
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string brnAddress { get; set; }
        public string branchNo { get; set; }

        public string docNo { get; set; }
        public string docDate { get; set; }
        public string docStatus { get; set; }
        public string licensePlate { get; set; }
        public string empCode { get; set; }
        public string empName { get; set; }
        public string useBrnCode { get; set; }
        public string useBrnName { get; set; }
        public string reasonDesc { get; set; }
        public string remark { get; set; }
        public decimal totalQty { get; set; }

        public int seqNo { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public string unitName { get; set; }
        public decimal itemQty { get; set; }

    }



    public class InvReceiveProdResponse
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }

        public string docDate { get; set; }
        public string docNo { get; set; }
        public string docStatus { get; set; }
        public string supCode { get; set; }
        public string supName { get; set; }
        public string invNo { get; set; }
        public string invDate { get; set; }
        public string vatType { get; set; }
        public string poNo { get; set; }
        public string remark { get; set; }
        public decimal itemQty { get; set; }
        public decimal subAmt { get; set; }

        public int seqNo { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public string unitName { get; set; }
        public decimal poQty { get; set; }
        public decimal itemRemain { get; set; }

    }



    public class InvTransferOutResponse
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }

        public string docDate { get; set; }
        public string docNo { get; set; }
        public string docStatus { get; set; }
        public string brnCodeTo { get; set; }
        public string brnNameTo { get; set; }
        public string refNo { get; set; }
        public int seqNo { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public string unitName { get; set; }
        public decimal itemQty { get; set; }

    }


    public class InvTransferInResponse
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string docDate { get; set; }
        public string docNo { get; set; }
        public string docStatus { get; set; }
        public string brnCodeFrom { get; set; }
        public string brnNameFrom { get; set; }
        public string refNo { get; set; }

        public int seqNo { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public string unitName { get; set; }
        public decimal itemQty { get; set; }
    }

    public class InvTransferCompareResponse
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }

        public string brnCodeOut { get; set; }
        public string docDateOut { get; set; }
        public string docNoOut { get; set; }
        public string brnCodeOutTo { get; set; }
        public decimal itemQtyOut { get; set; }

        public string brnCodeIn { get; set; }
        public string docDateIn { get; set; }
        public string docNoIn { get; set; }
        public decimal itemQtyIn { get; set; }
        public string remark { get; set; }
    }

    public class InvTransferNotInResponse
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }

        public string brnCodeFrom { get; set; }
        public string docDate { get; set; }
        public string docNo { get; set; }
        public string brnCodeTo { get; set; }

        public int seqNo { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public decimal itemQty { get; set; }
        public string remark { get; set; }
    }

    public class InvReturnSupResponse
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }

        public string docDate { get; set; }
        public string docNo { get; set; }
        public string docStatus { get; set; }
        public string supCode { get; set; }
        public string supName { get; set; }
        public string reasonDesc { get; set; }

        public int seqNo { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public string unitName { get; set; }
        public decimal itemQty { get; set; }
    }


    public class InvReturnOilResponse
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }

        public string docDate { get; set; }
        public string docNo { get; set; }
        public string docStatus { get; set; }

        public string brnCodeTo { get; set; }
        public string brnNameTo { get; set; }
        public string poNo { get; set; }
        public string reasonDesc { get; set; }

        public int seqNo { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public string unitName { get; set; }
        public decimal itemQty { get; set; }

    }


    public class InvAdjustResponse
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }

        public string docDate { get; set; }
        public string docNo { get; set; }
        public string docStatus { get; set; }
        public string docType { get; set; }

        public string empCode { get; set; }
        public string empName { get; set; }
        public string remark { get; set; }
        public string refNo { get; set; }
        public string reasonDesc { get; set; }
        public string brnCodeFrom { get; set; }
        public string brnNameFrom { get; set; }

        public int seqNo { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public string unitName { get; set; }
        public decimal itemQty { get; set; }

    }

}
