using Finance.API.Domain.Models.Request;
using Finance.API.Domain.Services.Communication;
using Finance.API.Models;

namespace Finance.API.Domain.Models.Response
{
    public class UpdateStatusExpenseResponse : BaseResponse<FinExpenseHd>
    {
        public UpdateStatusExpenseResponse(FinExpenseHd finExpense) : base(finExpense) { }

        public UpdateStatusExpenseResponse(string message) : base(message) { }
    }
}
