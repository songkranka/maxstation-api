using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Request
{
    public class SaveBranchTaxRequest
    {
        public MasBranchConfig MasBranchConfig { get; set; }
        public List<BranchTax> MasBranchTaxs { get; set; }
       
        public class BranchTax
        {
            public string CompCode { get; set; }
            public string BrnCode { get; set; }
            public decimal TaxAmt { get; set; }
            public string TaxId { get; set; }
            public string TaxName { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string UpdatedBy { get; set; }
        }
    }
}
