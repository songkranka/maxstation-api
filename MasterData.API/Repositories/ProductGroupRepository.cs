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
    public class ProductGroupRepository : SqlDataAccessHelper, IProductGroupRepository
    {
        public ProductGroupRepository(PTMaxstationContext context) : base(context)
        {

        }

        public List<MasProductGroup> GetProductGroupList(ProductGroupRequest req)
        {
            List<MasProductGroup> branchList = new List<MasProductGroup>();
            branchList = this.context.MasProductGroups.ToList();
            return branchList;
        }

        public async Task<QueryResult<MasProductGroup>> FindAllAsync(ProductGroupQuery query)
        {
            var queryable = context.MasProductGroups
                .OrderBy(x => x.GroupId)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.GroupName.Contains(query.Keyword)
                || p.GroupId.Contains(query.Keyword)
                );
            }

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<MasProductGroup>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }
    }
}
