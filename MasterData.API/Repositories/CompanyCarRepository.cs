using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
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
    public class CompanyCarRepository : SqlDataAccessHelper, ICompanyCarRepository
    {
        public CompanyCarRepository(PTMaxstationContext context) : base(context)
        {

        }

        public List<MasCompanyCar> GetCompanyCarLicenseList(LicensePlateRequest req)
        {
            var masCostCenters = context.MasCompanyCars.Where(
                x => (x.CompCode == req.CompCode && x.CarStatus == "Active")
            ).ToList();
            return masCostCenters;
        }

        public async Task<QueryResult<MasCompanyCar>> ListAsync(CompanyCarQuery query)
        {

            var queryable = context.MasCompanyCars
                .Where(x => x.CompCode == query.CompCode && x.CarStatus == "Active")
                .OrderBy(x => x.LicensePlate)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.LicensePlate.Contains(query.Keyword) 
                || p.CarRemark.Contains(query.Keyword));
            }

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<MasCompanyCar>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }
    }
}
