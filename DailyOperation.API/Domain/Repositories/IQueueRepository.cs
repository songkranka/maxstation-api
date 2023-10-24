using DailyOperation.API.Domain.Models;
using DailyOperation.API.Resources.POS;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Repositories
{
    public interface IQueueRepository
    {
        List<BranchSchedule> BranchScheduleList(BranchScheduleResource query);
        List<InfPosFunction2> GetPosFunction2(TranferPosResource query);
        List<InfPosFunction4> GetPosFunction4(TranferPosResource query);
        List<InfPosFunction5> GetPosFunction5(TranferPosResource query);
        List<InfPosFunction14> GetPosFunction14(TranferPosResource query);
        List<InfPosFunction2> CheckDuplicateFunction2(TranferPosResource query, List<InfPosFunction2> posFunction2List);
        List<InfPosFunction4> CheckDuplicateFunction4(TranferPosResource query, List<InfPosFunction4> posFunction4List);
        List<InfPosFunction5> CheckDuplicateFunction5(TranferPosResource query, List<InfPosFunction5> posFunction5List);
        List<InfPosFunction14> CheckDuplicateFunction14(TranferPosResource query, List<InfPosFunction14> posFunction14List);
        Task AddFunction2Async(List<InfPosFunction2> posFunction2List);
        Task AddFunction4Async(List<InfPosFunction4> posFunction4List);
        Task AddFunction5Async(List<InfPosFunction5> posFunction5List);
        Task AddFunction14Async(List<InfPosFunction14> posFunction14List);
    }
}
