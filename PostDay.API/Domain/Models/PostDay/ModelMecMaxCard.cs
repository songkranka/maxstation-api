using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Models.PostDay
{
    public class POSTPAID
    {
        public string maxcardno { get; set; }
        public string cusname { get; set; }
        public string cusno { get; set; }
        public string cusno_sap { get; set; }
        public decimal? total_baht { get; set; }
        public decimal? total_lite { get; set; }
    }

    public class PREPAID
    {
        public decimal? total_baht { get; set; }
        public decimal? total_lite { get; set; }
    }

    public class RESULT
    {
        public List<POSTPAID> POSTPAID { get; set; }
        public PREPAID PREPAID { get; set; }
        public USECOMP USECOMP { get; set; }
    }


    public class USECOMP
    {
        public decimal? total_baht { get; set; }
        public decimal? total_lite { get; set; }
    }
    public class ModelMecMaxCardResult
    {
        public RESULT RESULT { get; set; }
        public string CODE { get; set; }
        public string BRANCH { get; set; }
    }

    public class ModelMecMaxCardParam
    {
        public string date { get; set; }
        public string branch_id { get; set; }
        public string apikey { get; set; }
    }

    public class ModelMecPostPaidValidate
    {
        public string MecCuscode { get; set; }
        public string MaxCusCode { get; set; }
        public decimal? MecTotalBath { get; set; }
        public decimal? MaxCreditSaleSubAmt { get; set; }
        public bool IsValid { get; set; }

    }
}
