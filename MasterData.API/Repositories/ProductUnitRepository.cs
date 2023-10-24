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
    public class ProductUnitRepository : SqlDataAccessHelper, IProductUnitRepository
    {
        public ProductUnitRepository(PTMaxstationContext context) : base(context)
        {

        }

        public List<MasProductUnit> GetProductUnitList(ProductUnitRequest req)
        {
            //ตรวจสอบกรณีมีการส่ง PDBarcodeList มาใช้ IN
            List<string> pdBarcodeList = new List<string>();
            if (req.PDBarcodeList != null && req.PDBarcodeList != "")
            {
                pdBarcodeList = req.PDBarcodeList.Split(',').ToList();
            }

            List<MasProductUnit> resp = new List<MasProductUnit>();
            var sql = this.context.MasProductUnits.Where(
                    x => ((pdBarcodeList.Contains(x.UnitBarcode) || req.PDBarcodeList == null || req.PDBarcodeList == "")
                    )
                );
            resp = sql.ToList();
            return resp;
        }

        public async Task<MasProductUnit> FindByProductId(string productId)
        {
            return await context.MasProductUnits.AsNoTracking().FirstOrDefaultAsync(x => x.PdId == productId);
        }
    }
}
