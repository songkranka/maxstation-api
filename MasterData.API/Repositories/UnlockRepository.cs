using log4net;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MasterData.API.Resources.Unlock;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class UnlockRepository : SqlDataAccessHelper, IUnlockRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BranchRepository));

        public UnlockRepository(PTMaxstationContext context) : base(context) { }


        public async Task<List<EmployBranchConfigResponse>> GetEmployeeBranchConfig(EmployeeBranchConfigRequest req)
        {
            var empConfig = await (from e in context.AutEmployeeRoles
                                   join r in context.SysBranchConfigRoles on e.PositionCode equals r.PositionCode
                                   join c in context.MasBranchConfigDescs on r.ItemNo equals c.ItemNo
                                   where e.EmpCode == req.EmpCode && r.IsView == "Y"
                                   select new EmployBranchConfigResponse()
                                   {
                                       PositionCode = e.PositionCode,
                                       ItemNo = r.ItemNo,
                                       ConfigId = r.ConfigId,
                                       IsView = r.IsView,
                                       ConfigName = c.ConfigName,
                                       IsLockDate = c.IsLockDate
                                   }).ToListAsync();

            return empConfig;
        }

        public async Task<MasBranchConfig> GetIsLockMasBranchConfig(string compCode, string brnCode)
        {
            var sysBranchConfig = await context.MasBranchConfigs
                .FirstOrDefaultAsync(x => x.CompCode == compCode && x.BrnCode == brnCode);
            return sysBranchConfig;
        }

        public async Task<SysBranchConfigResponse> GetSysBranchConfig(SysBranchConfigRequest request)
        {
            var response = new SysBranchConfigResponse();

            var branch = (from b in this.context.MasBranches
                          join c in this.context.MasControls on new { b.CompCode, b.BrnCode } equals new { c.CompCode, c.BrnCode }
                          where b.BrnCode == request.BrnCode
                          && b.CompCode == request.CompCode
                          select new {b.BrnCode, b.BrnName, c.CtrlValue }).FirstOrDefault();

            var sysBranchConfig = await context.SysBranchConfigs
                .Where(p => p.CompCode == request.CompCode && p.BrnCode == request.BrnCode)
                .OrderBy(x=>x.DocDate).ThenBy(x=>x.SeqNo)
                .LastOrDefaultAsync();

            //var branch = await context.MasBranches.FirstOrDefaultAsync(x => x.BrnCode == request.BrnCode);
            var _curCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            var docDate = (branch == null)? DateTime.Now : DateTime.ParseExact(branch.CtrlValue, "dd/MM/yyyy", _curCulture);

            response.BrnCode = (branch == null)?"":branch.BrnCode;
            response.BrnName = (branch == null) ? "" : branch.BrnName;
            response.DocDate = docDate; 
            response.EmpName = (sysBranchConfig == null)? "": sysBranchConfig.EmpName;
            response.SeqNo = (sysBranchConfig == null)? 0: sysBranchConfig.SeqNo;
            response.LockDate = (sysBranchConfig == null) ? DateTime.Now : sysBranchConfig.CreatedDate;
            return response;
        }

        public async Task<List<SysBranchConfig>> GetSysBranchConfigListAsync(string compCode, string brnCode, string docDate)
        {
            var docDateQuery = DateTime.ParseExact(docDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var sysBranchConfig = await context.SysBranchConfigs.Where(p => p.CompCode == compCode && p.BrnCode == brnCode && p.DocDate == docDateQuery).ToListAsync();
            return sysBranchConfig;
        }

        public async Task AddUnlockAsync(SaveUnlockResource request)
        {
            var unlocks = request._Unlock;
            var sysBranch = context.SysBranchConfigs
                .Where(p => p.CompCode == request.CompCode && p.BrnCode == request.BrnCode && p.DocDate == request.DocDate)
                .OrderByDescending(x => x.SeqNo).FirstOrDefault();
            var seqNo = 1;

            if(sysBranch != null)
            {
                seqNo = sysBranch.SeqNo + 1;
            }

            foreach (var unlock in unlocks)
            {
                var sysBranchConfig = new SysBranchConfig();
                sysBranchConfig.CompCode = request.CompCode;
                sysBranchConfig.BrnCode = request.BrnCode;
                sysBranchConfig.DocDate = request.DocDate;
                sysBranchConfig.SeqNo = seqNo;
                sysBranchConfig.ItemNo = unlock.ItemNo;
                sysBranchConfig.ConfigId = unlock.ConfigId;
                sysBranchConfig.IsLock = unlock.IsLock;
                sysBranchConfig.EmpCode = request.EmpCode;
                sysBranchConfig.EmpName = context.MasEmployees.FirstOrDefaultAsync(x => x.EmpCode == request.EmpCode).Result.EmpName;
                sysBranchConfig.StartDate = unlock.StartDate;
                sysBranchConfig.EndDate = unlock.EndDate;
                sysBranchConfig.Remark = unlock.Remark;
                sysBranchConfig.CreatedDate = DateTime.Now;
                sysBranchConfig.CreatedBy = request.EmpCode;
                //seqNo++;

                await context.SysBranchConfigs.AddAsync(sysBranchConfig);
            }
        }
    }
}
