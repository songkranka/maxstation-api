using System;

namespace MasterData.API.Domain.Models.DocPattern.Request
{
    public class DocPatternRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string DocType { get; set; }
        public DateTime? DocDate { get; set; }
    }
}
