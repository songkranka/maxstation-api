using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class CustomerRepository : SqlDataAccessHelper, ICustomerRepository
    {
        public CustomerRepository(PTMaxstationContext context) : base(context)
        {

        }

        public List<MasCustomer> GetCustomerList(CustomerRequest req)
        {
            List<MasCustomer> customerList = new List<MasCustomer>();
            customerList = this.context.MasCustomers.Where(
                    x => (x.CustCode.Contains(req.Keyword) ||   x.CustName.Contains(req.Keyword) || x.Phone.Contains(req.Keyword) || x.Address.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                ).ToList();
            return customerList;
        }

        public async Task<QueryResult<MasCustomer>> ListAsync(CustomerQuery query)
        {
            var queryable = context.MasCustomers
                .OrderBy(x => x.CustCode)
                .AsNoTracking();

            switch (query?.ParentName ?? string.Empty)
            {
                case "Invoice":
                case "CreditSale":
                    queryable = queryable.Where(x => x.CustStatus == "Active");
                    break;
                default:
                    queryable = queryable.Where(x => x.CustStatus != "Cancel");
                    break;
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.CustCode.Contains(query.Keyword)
                    || p.CustName.Contains(query.Keyword)
                    || p.Phone.Contains(query.Keyword)
                    || p.Address.Contains(query.Keyword));
            }

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<MasCustomer>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }
        public async Task<MasCustomer> GetCustomerFromCitizenId(string pStrCitizenId)
        {
            pStrCitizenId = (pStrCitizenId ?? string.Empty).Trim();
            if (0.Equals(pStrCitizenId.Length))
            {
                return null;
            }
            Expression<Func<MasCustomer, bool>> exCustomer = null;
            exCustomer = x => pStrCitizenId.Equals( x.CitizenId) 
                && "Active".Equals(x.CustStatus)
            ;
            IQueryable<MasCustomer> qryCustomer = null;
            qryCustomer = context.MasCustomers.Where(exCustomer).AsNoTracking();
            MasCustomer result = null;
            result = await qryCustomer.FirstOrDefaultAsync();
            return result;
        }

        public async Task<QueryResult<MasCustomer>> FindAllAsync(CustomerQuery query)
        {
            var queryable = context.MasCustomers
                .OrderBy(x => x.CustCode)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.CustCode.Contains(query.Keyword)
                    || p.CustName.Contains(query.Keyword)
                    || p.Phone.Contains(query.Keyword)
                    || p.Address.Contains(query.Keyword));
            }

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<MasCustomer>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }
    }
}
