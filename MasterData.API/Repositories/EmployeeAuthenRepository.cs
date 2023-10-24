using log4net;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class EmployeeAuthenRepository : SqlDataAccessHelper, IEmployeeAuthenRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BranchRepository));

        public EmployeeAuthenRepository(PTMaxstationContext context) : base(context)
        {

        }

        public async Task<AutEmployeeRole> FindByEmpCodeAsync(string empCode)
        {
            return await context.AutEmployeeRoles.FirstOrDefaultAsync(x => x.EmpCode == empCode);
        }



    }
}
