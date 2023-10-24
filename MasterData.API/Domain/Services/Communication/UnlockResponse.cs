using MasterData.API.Resources.Unlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services.Communication
{
    public class UnlockResponse : BaseResponse<SaveUnlockResource>
    {
        public UnlockResponse(SaveUnlockResource unlock) : base(unlock) { }

        public UnlockResponse(string message) : base(message) { }
    }
}
