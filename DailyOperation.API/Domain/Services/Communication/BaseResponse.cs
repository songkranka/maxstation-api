using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Services.Communication
{
    public abstract class BaseResponse<T>
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public T Resource { get; private set; }
        public int TotalItems { get; private set; }

        protected BaseResponse(T resource, int totalItems = 0)
        {
            Success = true;
            Message = string.Empty;
            Resource = resource;
            TotalItems = totalItems;
        }

        protected BaseResponse(string message)
        {
            Success = false;
            Message = message;
            Resource = default;
        }
    }
}
