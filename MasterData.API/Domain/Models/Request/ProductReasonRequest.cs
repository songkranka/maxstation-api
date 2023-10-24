using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Request
{
    public class ProductReasonRequest
    {
        public string CompCode { get; set; }
        public string LocCode { get; set; }
        public string BrnCode { get; set; }
        public string ReasonId { get; set; }
        public string ReasonGroup { get; set; }
        public string Keyword { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
