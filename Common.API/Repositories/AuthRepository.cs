using Common.API.Domain.Repositories;
using Common.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Repositories
{
    public class AuthRepository : SqlDataAccessHelper, IAuthRepository
    {
        public AuthRepository(PTMaxstationContext context) : base(context) { }

        public async Task<MasEmployee> FindByEmpCodeAsync(string empCode)
        {
            var result = await context.MasEmployees.FirstOrDefaultAsync(x => x.EmpCode == empCode);
            return result;
        }

        public async Task<AutEmployeeRole> FindAuthEmpRoleByEmpCode(string empCode)
        {
            var result = await context.AutEmployeeRoles.FirstOrDefaultAsync(x => x.EmpCode == empCode);
            return result;
        }

        public async Task<MasBranch> FindBranchByBrnCode(string compCode, string empCode)
        {
            var branchQuery = (from e in context.MasEmployees
                          join o in context.MasOrganizes on e.CodeDev equals o.OrgCodedev
                          join b in context.MasBranches on new { A = o.OrgComp, B = o.OrgCode } equals new { A = b.CompCode, B = b.BrnCode }
                            where (e.EmpCode == empCode && o.OrgComp == compCode)
                            select  b).AsQueryable();
            var result = await branchQuery.FirstOrDefaultAsync();
            return result;
        }

        public Task<List<MasBranch>> GetBranchByCompCode(string compCode)
        {
            var branchList = context.MasBranches.Where(
                x => (x.CompCode == compCode)
            ).ToListAsync();
            return branchList;
        }

        public Task<List<MasBranch>> GetBranchByAuthCode(string compCode, int? authenCode)
        {
            var branchList = (from m in context.AutBranchRoles
                              join b in context.MasBranches on new { m.CompCode, m.BrnCode } equals new { b.CompCode, b.BrnCode }
                              where (m.AuthCode == authenCode && m.CompCode == compCode)
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
            return branchList;
        }

        public async Task<List<MasBranch>> GetAutBranchRole(string username, string compCode)
        {
            var response = new List<MasBranch>();
            var empRole = context.AutEmployeeRoles.FirstOrDefault(x => x.EmpCode == username);

            if (empRole != null)
            {
                //var authBranchRole = await context.AutBranchRoles.FirstOrDefaultAsync(x => x.AuthCode == empRole.AuthCode && x.CompCode == compCode);

                //if(authBranchRole != null)
                //{
                //    authBranchRoles = await context.MasBranches.Where(x => x.BrnCode == authBranchRole.BrnCode && x.CompCode == compCode).ToListAsync();
                //}
                var authBranchRoleQuery = (from br in context.AutBranchRoles.AsNoTracking()
                                     join mb in context.MasBranches.AsNoTracking()
                                     on br.BrnCode equals mb.BrnCode 
                                     where (br.AuthCode == empRole.AuthCode ) 
                                        && (br.CompCode == compCode)
                                        && (mb.CompCode == compCode)
                                           select new { mb }).AsQueryable();
                var authBranchRoles = await authBranchRoleQuery.ToListAsync();
                response = authBranchRoles.Select(x => new MasBranch
                {
                    CompCode = x.mb.CompCode,
                    BrnCode = x.mb.BrnCode,
                    LocCode = x.mb.LocCode,
                }).ToList();
            }

            return response;
        }

        public async Task<List<AutPositionRole>> GetAutPositionRole(string positionCode)
        {
            return  await context.AutPositionRoles.Where(x => x.PositionCode == positionCode).ToListAsync();
        }

        public async Task<MasPosition> GetMasPosition(string positionCode)
        {
            return await context.MasPositions.FirstOrDefaultAsync(x => x.PositionCode == positionCode);
        }

        public async Task InsertLogLogin(LogLogin pLogin)
        {
            await context.LogLogins.AddAsync(pLogin);
        } 
    }
}
