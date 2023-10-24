using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Response;
using Transferdata.API.Domain.Queries;
using Transferdata.API.Resources.Pos;

namespace Transferdata.API.Domain.Repositories
{
    public interface IPosRepository
    {
        Task<TranferPosResponse> GetDepositAmt(TranferPosQueryResource query);
    }
}
