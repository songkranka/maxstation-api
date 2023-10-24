using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MasterData.API.Resources.Product;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class ProductService : IProductService
    {
        private readonly ISoapPTWebServiceApi _soapPTWebServiceApi;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PTMaxstationContext _context;

        public ProductService(
            PTMaxstationContext context,
            ISoapPTWebServiceApi soapPTWebServiceApi,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _soapPTWebServiceApi = soapPTWebServiceApi;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }


        public List<Products> GetProductAllTypeList(ProductRequest req)
        {
            #region Comment for not get oil price

            //var oilPriceRequest = new OilPriceRequest
            //{
            //    brnCode = req.BrnCode,
            //};

            //OilPrice productUnits = null; //_soapPTWebServiceApi.GetOilPriceByBrn(oilPriceRequest);
            //List<Products> productList = null; // _productRepository.GetProductAllTypeList(req);
            //Action acGetProduct = () => { 
            //    productList = _productRepository.GetProductAllTypeList(req); 
            //};
            //Action acGetOilPrice = () => {
            //    productUnits = _soapPTWebServiceApi.GetOilPriceByBrn(oilPriceRequest).GetAwaiter().GetResult(); 
            //    //try
            //    //{
            //    //}
            //    //catch (Exception)
            //    //{

            //    //}
            //};
            ////Parallel.Invoke(new[] { acGetProduct, acGetOilPrice });
            //Task tkGetOilPrice = Task.Run(acGetOilPrice);
            //Task tkGetProduct = Task.Run(acGetProduct);
            //Task[] arrAllTask = new[] { tkGetOilPrice , tkGetProduct };
            //Task.WaitAll(arrAllTask);
            //if(productList == null 
            //|| !productList.Any() 
            //|| productUnits == null 
            //|| productUnits.data == null 
            //|| !productUnits.data.Any())
            //{
            //    return productList;
            //}
            //productList.ForEach(x =>
            //{
            //    foreach (var productUnit in productUnits.data)
            //    {
            //        string strUnitBarCode = x?.UnitBarcode?.Trim() ?? string.Empty;
            //        string strOpBarCode = productUnit?.opBarcode?.Trim() ?? string.Empty;
            //        if (!strUnitBarCode.Equals(strOpBarCode))
            //        {
            //            continue;
            //        }
            //        string strCurrentPrice = productUnit?.currentPrice?.Trim() ?? string.Empty;
            //        x.UnitPrice = decimal.Zero;
            //        decimal decUnitPrice = decimal.Zero;
            //        if (!string.IsNullOrEmpty(strCurrentPrice) && decimal.TryParse(strCurrentPrice, out decUnitPrice)) 
            //        {
            //            x.UnitPrice = decUnitPrice;
            //        }
            //        break;
            //    }
            //});

            var productList = _productRepository.GetProductAllTypeList(req);
            return productList;
            #endregion
        }
        /*
        public List<Products> GetProductAllTypeList(ProductRequest req)
        {
            var oilPriceRequest = new OilPriceRequest
            {
                brnCode = req.BrnCode,
            };

            var productUnits = _soapPTWebServiceApi.GetOilPriceByBrn(oilPriceRequest);
            var productList = _productRepository.GetProductAllTypeList(req);

            if (productList.Count > 0)
            {
                productList.ForEach(x =>
                {
                    //if (x.UnitBarcode == "000001")
                    //{
                    //    x.UnitPrice = decimal.Parse("26.04");
                    //}
                    //else if (x.UnitBarcode == "000004")
                    //{
                    //    x.UnitPrice = decimal.Parse("46.75");
                    //}
                    //else if (x.UnitBarcode == "000005")
                    //{
                    //    x.UnitPrice = decimal.Parse("27.20");
                    //}
                    foreach (var productUnit in productUnits.Result.data)
                    {
                        if (x.UnitBarcode.Trim() == productUnit.opBarcode.Trim())
                        {
                            x.UnitPrice = productUnit.currentPrice != null ? decimal.Parse(productUnit.currentPrice) : 0;
                        }
                    }
                });
            }

            return productList;
        }
        */

        public List<Products> GetProductList(ProductRequest req)
        {
            List<Products> productList = new List<Products>();
            productList = _productRepository.GetProductList(req);
            return productList;
        }

        public List<Products> GetProductListWithDocumentType(ProductRequest req)
        {
            List<Products> productList = new List<Products>();
            productList = _productRepository.GetProductListWithDocumentType(req);
            return productList;
        }

        public List<Products> GetProductServiceList(ProductRequest req)
        {
            List<Products> productList = new List<Products>();
            productList = _productRepository.GetProductServiceList(req);
            return productList;
        }

        public List<Products> GetProductListWithOutMaterialCode(ProductRequest req)
        {
            List<Products> productList = new List<Products>();
            productList = _productRepository.GetProductListWithOutMaterialCode(req);
            return productList;
        }

        public List<Products> GetProductReasonList(ProductReasonRequest req)
        {
            List<Products> productList = new List<Products>();
            productList = _productRepository.GetProductReasonList(req);
            return productList;
        }

        public async Task<QueryResult<MasProduct>> FindAllAsync(ProductQuery query)
        {
            return await _productRepository.FindAllAsync(query);
        }

        public async Task<MasProduct> FindByIdAsync(ProductResource query)
        {
            return await _productRepository.FindByIdAsync(query);
        }

        public async Task<MasProduct> FindProductOilTypeAsync(string pdId)
        {
            return await _productRepository.FindProductOilTypeAsync(pdId);
        }

        public async Task<ModelProduct> GetProduct(string pStrGuid)
        {
            pStrGuid = (pStrGuid ?? string.Empty).Trim();
            if (0.Equals(pStrGuid.Length))
            {
                return null;
            }
            Guid guid;
            if(!Guid.TryParse(pStrGuid , out guid))
            {
                return null;
            }
            var qryProduct = _context.MasProducts
                .Where(x => x.Guid == guid)
                .AsNoTracking();
            ModelProduct result = null;
            result = new ModelProduct();
            result.Product = await qryProduct.FirstOrDefaultAsync();

            if(result.Product != null)
            {
                var qryUnit = _context.MasProductUnits
                    .Where(x => x.PdId == result.Product.PdId)
                    .AsNoTracking();
                result.ArrProductUnit = await qryUnit.ToArrayAsync();

            }
            return result;
        }

        public async Task<ModelProduct> InsertProduct(ModelProduct param)
        {
            if(param == null || param.Product == null)
            {
                return null;
            }
            param.Product.Guid = Guid.NewGuid();
            param.Product.CreatedDate = DateTime.Now;
            await _context.MasProducts.AddAsync(param.Product);

            var qryProdUnit = _context.MasProductUnits
                .Where(x => x.PdId == param.Product.PdId)
                .AsNoTracking();
            _context.MasProductUnits.RemoveRange(qryProdUnit);
            if(param.ArrProductUnit != null && param.ArrProductUnit.Length > 0)
            {
                foreach (var item in param.ArrProductUnit)
                {
                    item.PdId = param.Product.PdId;
                    item.CreatedBy = param.Product.CreatedBy;
                    item.CreatedDate = param.Product.CreatedDate;                    
                }
                await _context.MasProductUnits.AddRangeAsync(param.ArrProductUnit);
            }
            await _unitOfWork.CompleteAsync();
            return param;
        }


        public async Task<ModelProduct> UpdateProduct(ModelProduct param)
        {
            if (param == null || param.Product == null)
            {
                return null;
            }
            EntityEntry entProduct = null;
            entProduct = _context.MasProducts.Attach(param.Product);
            entProduct.State = EntityState.Modified;
            param.Product.UpdatedDate = DateTime.Now;            
            var arrUnModified = new[] { 
                "CreatedBy", "CreatedDate", "Guid", 
                "PdId", "PdImage", "PdStatus" 
            };
            foreach (var item in arrUnModified)
            {
                entProduct.Property(item).IsModified = false;
            }
            var qryProdUnit = _context.MasProductUnits
                .Where(x => x.PdId == param.Product.PdId)
                .AsNoTracking();
            _context.MasProductUnits.RemoveRange(qryProdUnit);
            if (param.ArrProductUnit != null && param.ArrProductUnit.Length > 0)
            {
                foreach (var item in param.ArrProductUnit)
                {
                    item.PdId = param.Product.PdId;
                    item.CreatedBy = param.Product.CreatedBy;
                    item.CreatedDate = param.Product.CreatedDate;
                    item.UpdatedBy = param.Product.UpdatedBy;
                    item.UpdatedDate = param.Product.UpdatedDate;
                }
                await _context.MasProductUnits.AddRangeAsync(param.ArrProductUnit);
            }
            await _unitOfWork.CompleteAsync();
            return param;
        }


        public async Task<MasProduct> UpdateStatus(MasProduct param)
        {
            if (param == null)
            {
                return null;
            }
            var entProduct = _context.MasProducts.Attach(param);
            entProduct.State = EntityState.Unchanged;
            param.UpdatedDate = DateTime.Now;
            entProduct.Property(x => x.PdStatus).IsModified = true;
            await _unitOfWork.CompleteAsync();
            return param;
        }

        public async Task<bool> IsDuplicateProductId(string pStrProductId)
        {
            pStrProductId = (pStrProductId ?? string.Empty).Trim();
            if (0.Equals(pStrProductId.Length))
            {
                return true;
            }
            IQueryable<MasProduct> qryProduct = null;
            qryProduct = _context.MasProducts
                .Where(x => x.PdId == pStrProductId)
                .AsNoTracking();
            bool result = false;
            result = await qryProduct.AnyAsync();
            return result;
        }

        public async Task<MasUnit[]> GetAllUnit()
        {
            var qryUnit = _context.MasUnits
                .Where(x => x.UnitStatus == "Active")
                .AsNoTracking();
            MasUnit[] result = null;
            result = await qryUnit.ToArrayAsync();
            return result;
        }

        public async Task<List<MasProduct>> GetProductOilTypeAsync()
        {
            return await _productRepository.GetProductOilTypeAsync();
        }
    }
}
