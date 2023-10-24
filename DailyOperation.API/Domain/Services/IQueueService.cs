using DailyOperation.API.Domain.Models;
using DailyOperation.API.Resources.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Services
{
    public interface IQueueService
    {
        Task<TranferPos> TransferPOS(TranferPosResource query);
    }
}
