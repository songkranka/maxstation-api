using MasterData.API.Domain.Models.Dropdowns;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MasterData.API.Resources.CustomerCar;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class CustomerCarRepository : SqlDataAccessHelper, ICustomerCarRepository
    {
        public CustomerCarRepository(PTMaxstationContext context) : base(context)
        {

        }

        public QueryResult<CustomerCar> CustomerCarList(Resources.CustomerCar.CustomerCarQuery query)
        {
            var result = context.MasCustomerCars.Where(x => x.CustCode == query.CustomerCode).ToList();
            var customerCars = new List<CustomerCar>();
            customerCars = result.Select(x => new CustomerCar
            {
                Name = x.LicensePlate,
                Value = x.LicensePlate
            }).ToList();

            return new QueryResult<CustomerCar>
            {
                Items = customerCars
            };
        }

        public async Task<QueryResult<MasCustomerCar>> FindByCustCodeAsync(Domain.Models.Queries.CustomerCarQuery query)
        {
            var queryable = context.MasCustomerCars
                .Where(x => x.CustCode == query.CustCode && x.CarStatus == "Active")
                .OrderBy(x => x.LicensePlate)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.LicensePlate.Contains(query.Keyword));
            }

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<MasCustomerCar>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }

        public async Task<List<MasCustomerCar>> GetAllByCompany()
        {
            var customerCompanyQuery = (from p in context.MasCompanies
                                   join c in context.MasCustomerCars on p.CustCode equals c.CustCode
                                   where (p.CustCode != string.Empty)
                                   select c).AsQueryable();
            var result = await customerCompanyQuery.ToListAsync();
            return result;
        }
    }
}
