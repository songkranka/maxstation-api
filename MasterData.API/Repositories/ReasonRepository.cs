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
  
    public class ReasonRepository : SqlDataAccessHelper, IReasonRepository
    {

        public ReasonRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<QueryResult<MasReason>> ListAsync(ReasonQuery query)
        {
            var queryable =  this.context.MasReasons.AsQueryable() ;

            int totalItems = await queryable.CountAsync();
            var result =  await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<MasReason>
            {
                Items = result,
                TotalItems = totalItems,
            };
        }

        public async Task<QueryResult<MasReason>> WithdrawListAsync(WithdrawQuery query)
        {
            //var queryable = context.MasReasons
            //    .Where(x => x.ReasonGroup == "Withdraw" && x.ReasonStatus == "Active")
            //    .OrderBy(x => x.ReasonId)
            //    .AsNoTracking();

            var queryable = (from r in context.MasReasons
                             join rg in context.MasReasonGroups on new { r.ReasonId, r.ReasonGroup } equals new { rg.ReasonId, rg.ReasonGroup }
                             join p in context.MasProducts on rg.GroupId equals p.GroupId
                             where (r.ReasonStatus == "Active" && rg.ReasonGroup == "Withdraw" && p.PdId == query.PdId)
                             orderby r.ReasonId
                             select r).AsQueryable();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.ReasonId.Contains(query.Keyword) || p.ReasonDesc.Contains(query.Keyword));
            }

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<MasReason>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }

        public async Task<QueryResult<MasReason>> ProductReasonListAsync(ProductReasonQuery query)
        {
            var queryable = (from r in context.MasReasons
                             join rg in context.MasReasonGroups on new { r.ReasonId, r.ReasonGroup } equals new { rg.ReasonId, rg.ReasonGroup }
                             join p in context.MasProducts on rg.GroupId equals p.GroupId
                             where (r.ReasonStatus == "Active" && rg.ReasonGroup == query.ReasonGroup && p.PdId == query.PdId)
                             orderby r.ReasonId
                             select r).AsQueryable();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.ReasonId.Contains(query.Keyword) || p.ReasonDesc.Contains(query.Keyword));
            }

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<MasReason>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }
    }
}
