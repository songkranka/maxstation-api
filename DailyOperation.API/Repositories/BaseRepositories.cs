using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Repositories
{
    public class BaseRepositories
    {
        protected PTMaxstationContext Context;
        public BaseRepositories(PTMaxstationContext context)
        {
            this.Context = context;
        }

        //protected decimal ConvertStockQty(string pdid,string barcode,decimal itemqty)
        //{
        //    try
        //    {
        //        decimal stockqty = 0;
        //        var unit = this.Context.MasProductUnits.FirstOrDefault(x => x.PdId == x.PdId && x.UnitBarcode == barcode);
        //        if (unit != null)
        //        {
        //            decimal ratio = (unit.UnitStock / unit.UnitRatio) ?? 1;
        //            stockqty = ratio * itemqty;
        //        }
        //        return (stockqty==0)? itemqty:stockqty;
        //    }
        //    catch
        //    {
        //        return 1;
        //    }

        //}
    }
}
