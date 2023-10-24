using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Resources.Product;
using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface IProductRepository
    {
        List<Products> GetProductList(ProductRequest req);
        List<Products> GetProductListWithDocumentType(ProductRequest req);
        List<Products> GetProductAllTypeList(ProductRequest req);
        List<Products> GetProductListWithOutMaterialCode(ProductRequest req);
        List<Products> GetProductServiceList(ProductRequest req);
        List<Products> GetProductReasonList(ProductReasonRequest req);
        Task<QueryResult<MasProduct>> FindAllAsync(ProductQuery query);
        Task<MasProduct> FindByIdAsync(ProductResource query);
        Task<MasProduct> FindProductOilTypeAsync(string pdId);
        Task<List<MasProduct>> GetProductOilTypeAsync();
    }
}
