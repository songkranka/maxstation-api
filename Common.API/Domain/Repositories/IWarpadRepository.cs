using Common.API.Domain.Models;
using MaxStation.Entities.Models;
using System.Threading.Tasks;

namespace Common.API.Domain.Repositories
{
    public interface IWarpadRepository
    {
        Task<ResponseWarpadTaskList> GetWarpadTaskList(RequestWarpadTaskList req);
        string GetSysConfigApi(string systemId, string apiId);
    }
}
