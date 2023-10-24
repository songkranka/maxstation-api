using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services.Communication
{
    public enum EnumStatus
    {
        Failure, Success, TimeOut
    }
    public class ApiResponse<T>
    {

        public EnumStatus Status { get; set; }
        public T Result { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorStackTrace { get; set; }

        public void SetResult(T pResult)
        {
            this.Status = EnumStatus.Success;
            this.Result = pResult;
        }

        public void SetException(Exception pException)
        {
            if (pException == null)
            {
                return;
            }
            this.Status = EnumStatus.Failure;
            this.ErrorMessage = pException.Message;
            this.ErrorStackTrace = pException.StackTrace;
        }
    }
}
