using Abp.Linq.Expressions;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class SupplierService : ISupplierService
    {
        private PTMaxstationContext _context = null;
        private IUnitOfWork _unitOfWorf = null;

        public SupplierService(
            PTMaxstationContext pContext, 
            IUnitOfWork pUnitOfWork
        ){
            _context = pContext;
            _unitOfWorf = pUnitOfWork;

        }

        public async Task<ModelGetSupplierListResult> GetSupplierList(ModelGetSupplierListParam param)
        {
            if(param == null)
            {
                return null;
            }
            ModelGetSupplierListResult result = null;
            result = new ModelGetSupplierListResult();
            
            string strKeyWord;
            strKeyWord  = (param.KeyWord ?? string.Empty).Trim();

            IQueryable<MasSupplier> qrySupplier = null;
            qrySupplier = _context.MasSuppliers.AsNoTracking();

            if (strKeyWord.Length > 0)
            {
                ExpressionStarter<MasSupplier> esSupplier = null;
                esSupplier = PredicateBuilder.New<MasSupplier>(x => false);

                esSupplier = esSupplier.Or(x => x.SupCode.Contains(strKeyWord));
                //esSupplier = esSupplier.Or(x => strKeyWord.Contains( x.SupCode));
                esSupplier = esSupplier.Or(x => x.SupPrefix.Contains(strKeyWord));
                //esSupplier = esSupplier.Or(x => strKeyWord.Contains(x.SupPrefix));
                esSupplier = esSupplier.Or(x => x.SupName.Contains(strKeyWord));
                //esSupplier = esSupplier.Or(x => strKeyWord.Contains(x.SupName));
                //esSupplier = esSupplier.Or(x => x.SupName.Contains(strKeyWord));
                qrySupplier = qrySupplier.Where(esSupplier);
            }

            result.TotalItem = await qrySupplier.CountAsync();
            if (param.ItemPerPage > 0 && param.PageIndex > 0)
            {
                qrySupplier = qrySupplier
                    .Skip((param.PageIndex - 1) * param.ItemPerPage)
                    .Take(param.ItemPerPage);
            }
            result.ArrSuplier = await qrySupplier.ToArrayAsync();
            return result;
        }

        public async Task<ModelSupplier> GetSupplier(string pStrGuid)
        {
            pStrGuid = (pStrGuid ?? string.Empty).Trim();
            if(pStrGuid.Length == 0)
            {
                return null;
            }

            Guid gid;
            if(!Guid.TryParse(pStrGuid , out gid))
            {
                return null;
            }
            IQueryable<MasSupplier> qrySupp = null;
            qrySupp = _context.MasSuppliers.Where(
                x => x.Guid == gid
            ).AsNoTracking();



            ModelSupplier result = null;
            result = new ModelSupplier();

            result.Supplier = await qrySupp.FirstOrDefaultAsync();

            if(result.Supplier != null)
            {
                IQueryable<MasSupplierProduct> qrySupProduct = null;
                qrySupProduct = _context.MasSupplierProducts.Where(
                    x => x.SupCode == result.Supplier.SupCode
                ).AsNoTracking();

                result.ArrSupProduct = await qrySupProduct.ToArrayAsync();
            }

            


            return result;
        }

        public async Task<bool> CheckDuplicateCode(string pStrSupCode)
        {
            pStrSupCode = (pStrSupCode ?? string.Empty).Trim();
            if(pStrSupCode.Length == 0)
            {
                return false;
            }

            IQueryable<MasSupplier> qrySupp = null;
            qrySupp = _context.MasSuppliers.Where(
                x => x.SupCode == pStrSupCode
            ).AsNoTracking();

            bool result;
            result = await qrySupp.AnyAsync();

            return result;
        }

        public async Task<MasSupplier> UpdateStatus(MasSupplier param)
        {
            if (param == null)
            {
                return null;
            }
            EntityEntry<MasSupplier> entSupp;
            entSupp = _context.Attach(param);
            entSupp.State = EntityState.Unchanged;
            param.UpdatedDate = DateTime.Now;
            entSupp.Property(x => x.UpdatedBy).IsModified = true;
            entSupp.Property(x => x.SupStatus).IsModified = true;
            await _unitOfWorf.CompleteAsync();
            return param;
        }

        public async Task<ModelSupplier> InsertSupplier(ModelSupplier param)
        {
            if (param == null)
            {
                return null;
            }
            MasSupplier supp;
            supp = param.Supplier;
            if (supp != null)
            {
                supp.Guid = Guid.NewGuid();
                supp.CreatedDate = DateTime.Now;
                await _context.AddAsync(supp);

                IQueryable<MasSupplierProduct> qrySuppProduct;

                qrySuppProduct = _context.MasSupplierProducts
                    .Where(x => x.SupCode == supp.SupCode)
                    .AsNoTracking();
                _context.MasSupplierProducts.RemoveRange(qrySuppProduct);

                MasSupplierProduct[] arrSupProd;
                arrSupProd = param.ArrSupProduct;
                if (arrSupProd != null && arrSupProd.Length > 0)
                {
                    arrSupProd = arrSupProd.Where(x => x != null).ToArray();
                }
                if (arrSupProd != null && arrSupProd.Length > 0)
                {
                    
                    await _context.MasSupplierProducts.AddRangeAsync(arrSupProd);
                    param.ArrSupProduct = arrSupProd;
                }
                await _unitOfWorf.CompleteAsync();

            }
            return param;
        }

        public async Task<ModelSupplier> UpdateSupplier(ModelSupplier param)
        {
            if (param == null)
            {
                return null;
            }
            MasSupplier sup;
            sup = param.Supplier;
            
            if (sup != null)
            {
                sup.UpdatedDate = DateTime.Now;

                EntityEntry<MasSupplier> entSup = null;
                entSup = _context.Attach(sup);
                entSup.State = EntityState.Modified;                
                entSup.Property(x => x.CreatedBy).IsModified = false;
                entSup.Property(x => x.CreatedDate).IsModified = false;
                entSup.Property(x => x.SupCode).IsModified = false;
                entSup.Property(x => x.Guid).IsModified = false;
                //entSup.Property(x => x.MapSupCode).IsModified = false;
                entSup.Property(x => x.SupStatus).IsModified = false;

                IQueryable<MasSupplierProduct> qrySupProd = null;
                qrySupProd = _context.MasSupplierProducts
                    .Where(x => x.SupCode == sup.SupCode)
                    .AsNoTracking();
                _context.RemoveRange(qrySupProd);
                
            }
            MasSupplierProduct[] arrSupProd;
            arrSupProd = param.ArrSupProduct;
            if (arrSupProd != null && arrSupProd.Length > 0)
            {
                arrSupProd = arrSupProd.Where(x => x != null).ToArray();
            }
            if (arrSupProd != null && arrSupProd.Length > 0)
            {

                await _context.AddRangeAsync(arrSupProd);
                param.ArrSupProduct = arrSupProd;
            }
            await _unitOfWorf.CompleteAsync();
            return param;            
        }

    }
}
