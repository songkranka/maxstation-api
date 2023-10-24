using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Resources.Transferdata;

namespace Transferdata.API.Domain.Repositories
{
    public interface IWarpadRepository
    {
        Task<WarpadCloseDay> SendCloseDay(TransferDataResource req);
    }
}
