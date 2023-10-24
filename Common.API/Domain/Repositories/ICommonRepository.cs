using MaxStation.Entities.Models;
using Common.API.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.API.Domain.Repositories
{
    public interface ICommonRepository
    {
        void UpdateCloseDay(RequestData req);
    }
}
