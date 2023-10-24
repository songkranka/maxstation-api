using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class ProductGroupService : IProductGroupService
    {
        private readonly IProductGroupRepository _productGroupRepository;

        public ProductGroupService(
            IProductGroupRepository productGroupRepository)
        {
            _productGroupRepository = productGroupRepository;
        }

        public List<MasProductGroup> GetProductGroupList(ProductGroupRequest req)
        {
            List<MasProductGroup> productGroupList = new List<MasProductGroup>();
            productGroupList = _productGroupRepository.GetProductGroupList(req);
            return productGroupList;
        }

        public async Task<QueryResult<MasProductGroup>> FindAllAsync(ProductGroupQuery query)
        {
            return await _productGroupRepository.FindAllAsync(query);
        }
    }
}
