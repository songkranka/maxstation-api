using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Repositories;
using Inventory.API.Helpers;
using Inventory.API.Services;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Inventory.API.Repositories
{
    public class PurchaseOrderRepository : SqlDataAccessHelper, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(PTMaxstationContext context) : base(context) { }


        public async Task<PurchaseOrder> ListAsync(PurchaseOrderHdQuery query)
        {
            var response = new PurchaseOrder();

            string strComCode = DefaultService.EncodeSqlString(query?.CompCode);
            string strPlant = DefaultService.EncodeSqlString(query?.BranchCode);
            string strDocDate = query?.SystemDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");
            
            if(0.Equals(strComCode.Length) || 0.Equals(strPlant.Length))
            {
                return response;
            }

            string strSql = $@"select distinct h.PO_NUMBER, h.COMP_CODE, h.CREAT_DATE, h.CREATED_BY, h.CREATED_BY_1, h.CREATED_DATE_1, h.CURRENCY, h.DELETE_IND, h.DOC_DATE, h.DOC_TYPE
                            , h.DOWNPAY_AMOUNT, h.DOWNPAY_DUEDATE, h.DOWNPAY_PERCENT, h.DOWNPAY_TYPE, h.END_DATE, h.ERROR_MSG, h.INF_STATUS, h.IS_DELETED, h.PMNTTRMS, h.PUR_GROUP
                            , h.PURCH_ORG, h.RECEIVE_BRN_CODE, h.RECEIVE_DOC_NO, h.RECEIVE_LOC_CODE, h.RECEIVE_STATUS, h.RECEIVE_UPDATE, h.RETENTION_PERCENTAGE, h.RETENTION_TYPE, h.START_DATE, h.STATUS,
                            h.UPDATED_BY, h.UPDATED_DATE, h.VENDOR
                            ,s.SUP_NAME as SupplierName
                            from INF_PO_HEADER h
                            left join INF_PO_ITEM i
                            on h.PO_NUMBER = i.PO_NUMBER
                            and isnull(i.RET_ITEM,'') <> 'X'
                            inner join MAS_SUPPLIER s
                            on h.VENDOR = s.SUP_CODE
                            inner join INF_PO_TYPE t
                            on h.DOC_TYPE = t.PO_TYPE_ID
                            where t.PO_TYPE_DESC <> 'Gas' 
                            and h.COMP_CODE = '{strComCode}'
                            and i.PLANT = '{strPlant}'
                            and h.DOC_DATE >= dateadd(day,-30,'{strDocDate}') 
                            and isnull(h.DELETE_IND,'') <> 'L'
                            and isnull(h.RECEIVE_STATUS,'') <> 'Y'";
            response.InfPoHeaders = await DefaultService.GetEntityFromSql<List<InfPoHeader>>(context, strSql);
            return response;
        }

        public async Task<PurchaseOrder> ListAsyncOld(PurchaseOrderHdQuery query)
        {
            var systemDate = query.SystemDate;
            var gasDocType = "Gas";
            var response = new PurchaseOrder();

            var queryable = (from hd in context.InfPoHeaders
                              from dt in context.InfPoItems.Where(x => hd.PoNumber == x.PoNumber && x.RetItem != "X")
                                                           .Take(1)
                                                           .DefaultIfEmpty()
                             join type in context.InfPoTypes on hd.DocType equals type.PoTypeId
                             where hd.CompCode == query.CompCode && dt.Plant == query.BranchCode 
                                    //&& hd.DocDate <= query.SystemDate
                                    && hd.DeleteInd != "L" && hd.ReceiveStatus != "Y" 
                                    && !gasDocType.Equals(type.PoTypeDesc)
                             select hd);

            response.InfPoHeaders = await queryable.ToListAsync();
            response.InfPoHeaders.ForEach(x => {
                var masSupplier = context.MasSuppliers.FirstOrDefault(y => y.SupCode == x.Vendor);
                x.SupplierName = masSupplier == null ? string.Empty : masSupplier.SupName;
                //x.SupplierName = context.MasSuppliers.FirstOrDefault(y => y.SupCode == x.Vendor).SupName ?? string.Empty;
                //x.DocDate = context.InfPoItems.FirstOrDefault(y => y.PoNumber == x.PoNumber).PoDate ?? x.DocDate;
            });

            return response;
        }
        /*
         public async Task<PurchaseOrder> ListAsync(PurchaseOrderHdQuery query)
        {
            var systemDate = query.SystemDate;
            var gasDocType = "Gas";
            var response = new PurchaseOrder();

            var queryable = await (from hd in context.InfPoHeaders
                              from dt in context.InfPoItems.Where(x => hd.PoNumber == x.PoNumber)
                                                           .Take(1)
                                                           .DefaultIfEmpty()
                             join type in context.InfPoTypes on hd.DocType equals type.PoTypeId
                             where hd.CompCode == query.CompCode && dt.Plant == query.BranchCode && hd.DocDate <= query.SystemDate
                                   && hd.DeleteInd != "L" && hd.ReceiveStatus != "Y" && !gasDocType.Equals(type.PoTypeDesc)
                             select hd).ToListAsync();

            response.InfPoHeaders = queryable;
            response.InfPoHeaders.ForEach(x => {
                x.SupplierName = context.MasSuppliers.FirstOrDefault(y => y.SupCode == x.Vendor).SupName ?? string.Empty;
            });

            return response;
        }
         */
        public async Task<PurchaseOrder> DetailListAsync(PurchaseOrderHdQuery query)
        {
            var response = new PurchaseOrder();
            var poItems = await context.InfPoItems
                .Where(x => x.PoNumber == query.PoNumber && x.DeleteInd != "L")
                .ToListAsync();
            List<string> materialIds = poItems.Select(m => m.Material).ToList();
            var products = await context.MasProducts
                .Where(x => materialIds.Contains(x.PdId))
                .ToListAsync();

            if(products.Count > 0)
            {
                products.ForEach(x =>
                {
                    var masUnit = context.MasUnits.FirstOrDefault(u => u.UnitId == x.UnitId);
                    x.UnitName = masUnit != null ? masUnit.UnitName : string.Empty;
                });
            }

            response.InfPoItems = poItems;
            response.MasProducts = products;
            response.MasUnits = await getListMasUnit(poItems);
            return response;
        }

        private async Task< List<MasUnit>> getListMasUnit(List<InfPoItem> pListPoItem)
        {
            if(pListPoItem == null || !pListPoItem.Any())
            {
                return null;
            }
            string[] arrMapUnitId = pListPoItem
                .Select(x => (x?.PoUnit ?? string.Empty).Trim())
                .Where(x=> !0.Equals(x.Length))
                .Distinct()
                .ToArray();
            if (arrMapUnitId == null || !arrMapUnitId.Any())
            {
                return null;
            }
            Expression<Func<MasUnit, bool>> expUnit = null;
            expUnit = x => arrMapUnitId.Contains(x.MapUnitId) 
                && "Active".Equals(x.UnitStatus);
            IQueryable<MasUnit> qryUnit = null;
            qryUnit = context.MasUnits.Where(expUnit).AsNoTracking();
            List<MasUnit> result = null;
            result = await qryUnit.ToListAsync();
            return result;
        }
    }

    
}
