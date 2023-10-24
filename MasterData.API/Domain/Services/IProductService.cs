using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Resources.Product;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IProductService
    {
        List<Products> GetProductList(ProductRequest req);
        List<Products> GetProductListWithDocumentType(ProductRequest req);
        List<Products> GetProductAllTypeList(ProductRequest req);
        List<Products> GetProductServiceList(ProductRequest req);
        List<Products> GetProductListWithOutMaterialCode(ProductRequest req);
        List<Products> GetProductReasonList(ProductReasonRequest req);
        Task<QueryResult<MasProduct>> FindAllAsync(ProductQuery query);
        Task<MasProduct> FindByIdAsync(ProductResource req);
        Task<MasProduct> FindProductOilTypeAsync(string pdId);
        Task<ModelProduct> GetProduct(string pStrGuid);
        Task<ModelProduct> InsertProduct(ModelProduct param);
        Task<ModelProduct> UpdateProduct(ModelProduct param);
        Task<MasProduct> UpdateStatus(MasProduct param);
        Task<bool> IsDuplicateProductId(string pStrProductId);
        Task<MasUnit[]> GetAllUnit();
        Task<List<MasProduct>> GetProductOilTypeAsync();

    }
}
