using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Resources.Unlock
{
    public class SaveUnlockResource
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime DocDate { get; set; }
        public string EmpCode { get; set; }
        public string CreatedBy { get; set; }
        
        public List<Unlock> _Unlock { get; set; }
        public class Unlock
        {
            public int ItemNo { get; set; }
            public string ConfigId { get; set; }
            public string IsLock { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Remark { get; set; }
            
        }
    }
}
