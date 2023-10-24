using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Resources.CompanyCar
{
    public class CompanyCarQueryReqource : QueryResource
    {
        public string CompCode { get; set; }
        public string Keyword { get; set; }
    }
}
