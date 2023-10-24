using MasterData.API.Domain.Models.Queries;
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
    public class EmployeeRepository :  SqlDataAccessHelper, IEmployeeRepository
    {
        public EmployeeRepository(PTMaxstationContext context) : base(context) { }

        public async Task<MasEmployee> FindByIdAsync(string id)
        {
            var employee = await context.MasEmployees.AsNoTracking().FirstOrDefaultAsync(x => x.EmpCode == id);

            return employee;
        }

        public async Task<QueryResult<MasEmployee>> ListAsync(EmployeeQuery query)
        {
            var queryable = context.MasEmployees
                .OrderBy(x => x.EmpCode)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.PersonFnameThai.Contains(query.Keyword)
                            || p.PersonLnameThai.Contains(query.Keyword)
                            || p.EmpCode.Contains(query.Keyword));
            }

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<MasEmployee>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }

        public async Task<QueryResult<MasEmployee>> ListAllWitnoutPageAsync(EmployeeQuery query)
        {
            var queryable = context.MasEmployees
                .OrderBy(x => x.EmpCode)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.PersonFnameThai.Contains(query.Keyword)
                            || p.PersonLnameThai.Contains(query.Keyword)
                            || p.EmpCode.Contains(query.Keyword));
            }

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.ToListAsync();

            return new QueryResult<MasEmployee>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }


        public async Task<QueryResult<MasEmployee>> ListAllByBranch(EmployeeQueryByBranch query)
        {
            var resp = await (from emp in context.MasEmployees
                              join org in context.MasOrganizes on emp.CodeDev equals org.OrgCodedev
                              where org.OrgCode == query.BrnCode select emp).AsNoTracking().ToListAsync();

            return new QueryResult<MasEmployee>
            {
                Items = resp,
            };
        }
    }
}
