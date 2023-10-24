using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface IProductGroupRepository
    {
        List<MasProductGroup> GetProductGroupList(ProductGroupRequest req);
        Task<QueryResult<MasProductGroup>> FindAllAsync(ProductGroupQuery query);
    }
}
