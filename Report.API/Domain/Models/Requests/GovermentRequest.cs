using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Requests
{
    public class GovermentRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public eBranchType BranchType { get; set; }
        public eUnitType UnitType { get; set; }

        public enum eUnitType
        {
            Liter = 0,
            Kilo = 1
        }
        public enum eBranchType
        {
            Single = 0,
            Multi = 1
        }

    }



}
