using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
  
    public class BankRepository : SqlDataAccessHelper, IBankRepository
    {

        public BankRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<QueryResult<MasBank>> ListAsync(BankQuery query)
        {

            var queryable =  this.context.MasBanks.Where(x => 
                (x.CompCode == query.CompCode || query.CompCode == null || query.CompCode == "") 
                && (x.BankCode.Contains(query.Keyword) || x.BankName.Contains(query.Keyword) || x.AccountNo.Contains(query.Keyword) || query.Keyword == "" || query.Keyword == null)
            ).AsQueryable() ;

            int totalItems = await queryable.CountAsync();
            var result = await  queryable.ToListAsync();

            return new QueryResult<MasBank>
            {
                Items = result,
                TotalItems = totalItems,
            };
        }
    }
}
