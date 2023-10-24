using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;

namespace Transferdata.API.Domain.Repositories
{
    public interface IMeterRepository 
    {
        Task<DopPeriod> GetPeriodAsync(PeriodQuery query);

    }

}
