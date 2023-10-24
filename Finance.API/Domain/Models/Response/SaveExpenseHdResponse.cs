using Finance.API.Domain.Models.Request;
using Finance.API.Domain.Services.Communication;
using Finance.API.Models;

namespace Finance.API.Domain.Models.Response
{
    public class SaveExpenseResponse : BaseResponse<SaveExpenseRequest>
    {
        public SaveExpenseResponse(SaveExpenseRequest finExpense) : base(finExpense) { }

        public SaveExpenseResponse(string message) : base(message) { }
    }
}
