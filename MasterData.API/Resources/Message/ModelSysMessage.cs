using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Resources.Message
{
    public class ModelSysMessage
    {
        public int MsgCode { get; set; }
        public string MsgLang { get; set; }
        public string MsgText { get; set; }
    }
}
