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
    public class ProductUnitService : IProductUnitService
    {
        private readonly IProductUnitRepository _productUnitRepository;

        public ProductUnitService(
            IProductUnitRepository productUnitRepository)
        {
            _productUnitRepository = productUnitRepository;
        }

        public List<MasProductUnit> GetProductUnitList(ProductUnitRequest req)
        {
            List<MasProductUnit> resp = new List<MasProductUnit>();
            resp = _productUnitRepository.GetProductUnitList(req);
            return resp;
        }
    }
}
