using Common.API.Domain.Models;
using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.API.Domain.Services
{
    public interface ICommonService
    {
        Task<string> UpdateCloseDay(RequestData req);
        Task<SysNotification[]> GetNoti(GetNotiParam param);
        Task<bool> UpdateNoti(SysNotification param);
    }
}
