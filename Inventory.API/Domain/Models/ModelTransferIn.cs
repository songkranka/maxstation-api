using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Domain.Models
{
    //public class ModelTransferIn
    //{
    //}

    public class ModelTransferInHeader : InvTraninHd
    {
        public List<InvTraninDt> ListTransferInDetail { get; set; }

        public async Task LoadDetailAsync(PTMaxstationContext pContex)
        {
            if(pContex == null)
            {
                return;
            }
            ListTransferInDetail = await pContex.InvTraninDts.Where(
                x => x.BrnCode == BrnCode
                && x.CompCode == CompCode
                && x.DocNo == DocNo
                && x.LocCode == LocCode
            ).ToListAsync();
        }
    }

    public class ModelResponseData<T>
    {
        public string ErrorStackTrace { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public T Data { get; set; } = default(T);
        public bool IsSuccess { get; set; } = true;
        public int totalItems { get; set; } = 0;

        public void SetException(Exception pException)
        {
            if(pException == null)
            {
                return;
            }
            IsSuccess = false;
            ErrorStackTrace = pException.StackTrace;
            while (pException.InnerException != null) pException = pException.InnerException;
            ErrorMessage = pException.Message;
        }

        public void SetData(T pData)
        {
            Data = pData;
            IsSuccess = true;
        }
    }
}
