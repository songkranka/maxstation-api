using System;

namespace Finance.API.Domain.Models.Request
{
    public class DocPatternRequest
    {
          public string CompCode { get; set; }
          public string BrnCode { get; set; }
          public string DocType { get; set; }
          public DateTime DocDate { get; set; }
    }
}
