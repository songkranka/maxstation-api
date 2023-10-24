using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Resources;
using Transferdata.API.Resources.Transferdata;

namespace Transferdata.API.Domain.Services
{
    public interface IWarpadService
    {
        Task<WarpadCloseDay> SendCloseDay(TransferDataResource query);
    }
}
