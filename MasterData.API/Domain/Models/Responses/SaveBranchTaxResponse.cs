using MasterData.API.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses
{
    public class SaveBranchTaxResponse : BaseResponse<BranchTaxResponse>
    {
        public SaveBranchTaxResponse(BranchTaxResponse branchTaxResponse) : base(branchTaxResponse) { }

        public SaveBranchTaxResponse(string message) : base(message) { }
    }
}
