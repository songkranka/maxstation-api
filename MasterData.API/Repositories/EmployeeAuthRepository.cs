using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Responses;
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
    public class EmployeeAuthRepository : SqlDataAccessHelper, IEmployeeAuthRepository
    {
        public EmployeeAuthRepository(PTMaxstationContext context) : base(context)
        {

        }
        public async Task<AutEmployeeRole> FindByEmpCodeAsync(string empCode)
        {
            var employee = await context.AutEmployeeRoles.AsNoTracking().FirstOrDefaultAsync(x => x.EmpCode == empCode);

            return employee;
        }

        public async Task AddAuthEmployeeRoleAsync(AutEmployeeRole authEmployeeRole)
        {
            await context.AutEmployeeRoles.AddAsync(authEmployeeRole);
        }

        public async Task UpdateAuthEmployeeRoleAsync(AutEmployeeRole authEmployeeRole)
        {
            var result = await context.AutEmployeeRoles.SingleOrDefaultAsync(x => x.EmpCode == authEmployeeRole.EmpCode);
            if (result != null)
            {
                result.AuthCode = authEmployeeRole.AuthCode;
                result.PositionCode = authEmployeeRole.PositionCode;
                result.UpdatedDate = authEmployeeRole.UpdatedDate;
                result.UpdatedBy = authEmployeeRole.UpdatedBy;
                context.SaveChanges();
            }
        }

        public async Task<MasBranch> FindBranchByBrnCode(string compCode, string empCode)
        {
            var branchQuery = (from e in context.MasEmployees
                               join o in context.MasOrganizes on e.CodeDev equals o.OrgCodedev
                               join b in context.MasBranches on new { A = o.OrgComp, B = o.OrgCode } equals new { A = b.CompCode, B = b.BrnCode }
                               where (e.EmpCode == empCode && o.OrgComp == compCode)
                               select b).AsQueryable();
            var result = await branchQuery.FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<MasBranch>> GetBranchByCompCode(string compCode)
        {
            var branchList = context.MasBranches.Where(
                x => (x.CompCode == compCode)
            ).ToListAsync();
            return await branchList;
        }

        public async Task<List<MasBranch>> GetBranchByAuthCode(string compCode, int? authCode)
        {
            var branchList = (from m in context.AutBranchRoles
                              join b in context.MasBranches on new { m.CompCode, m.BrnCode } equals new { b.CompCode, b.BrnCode }
                              where (m.AuthCode == authCode && m.CompCode == compCode)
                              select new MasBranch()
                              {
                                  CompCode = b.CompCode,
                                  BrnCode = b.BrnCode,
                                  MapBrnCode = b.MapBrnCode,
                                  LocCode = b.LocCode,
                                  BrnName = b.BrnName,
                                  BrnStatus = b.BrnStatus,
                                  BranchNo = b.BranchNo,
                                  Address = b.Address,
                                  SubDistrict = b.SubDistrict,
                                  District = b.District,
                                  Province = b.Province,
                                  Postcode = b.Postcode,
                                  Phone = b.Phone,
                                  Fax = b.Fax,
                                  CreatedDate = b.CreatedDate,
                                  CreatedBy = b.CreatedBy,
                                  UpdatedBy = b.UpdatedBy,
                                  UpdatedDate = b.UpdatedDate,
                              }).ToListAsync();
            return await branchList;
        }
    }
}
