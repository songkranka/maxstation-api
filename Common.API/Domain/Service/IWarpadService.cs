using Common.API.Domain.Models;
using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.API.Domain.Services
{
    public interface IWarpadService
    {
        Task<ResponseWarpadTaskList> GetWarpadTaskList(RequestWarpadTaskList req);

        Task<ModelGetToDoTaskResult> GetToDoTask(string pStrEmpCode);
    }
}
