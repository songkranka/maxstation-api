using System.Collections.Generic;

namespace MasterData.API.Domain.Models.Request
{
    public class SaveUnlock
    {
        public string PositionCode { get; set; }
        public List<UnlockPosition> _UnlockPosition { get; set; }

        public class UnlockPosition
        {
            public int ItemNo { get; set; }
            public string ConfigId { get; set; }
            public string ConfigName { get; set; }
            public string IsView { get; set; }
        }
    }
}
