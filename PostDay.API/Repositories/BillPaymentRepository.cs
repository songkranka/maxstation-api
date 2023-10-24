using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using PostDay.API.Domain.Models;
using PostDay.API.Domain.Repositories;
using PostDay.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Repositories
{
    public class BillPaymentRepository : SqlDataAccessHelper,IBillPaymentRepository
    {
        private readonly PTMaxstationContext _context = null;

        public BillPaymentRepository(PTMaxstationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<MasBank>> GetBankList()
        {
            var qryBank = _context.MasBanks.AsNoTracking();
            var result = await qryBank.ToListAsync();

            return result;
        }

        public async Task<GetPostDayResult> GetPostDay(GetPostDayParam param)
        {
            var result = new GetPostDayResult();
            result.IsSuccess = false;
            if (param == null)
            {
                result.Message = "Parameter Is Null";
                return result;
            }
            var qryDopPostDay = _context.DopPostdayHds
                .Where(x => x.BillPayment != "Y")
                .AsNoTracking();
            param.CompCode = (param.CompCode ?? string.Empty).Trim();
            if (!0.Equals(param.CompCode.Length))
            {
                qryDopPostDay = qryDopPostDay.Where(x => x.CompCode == param.CompCode);
            }
            param.BrnCode = (param.BrnCode ?? string.Empty).Trim();
            if (!0.Equals(param.BrnCode.Length))
            {
                qryDopPostDay = qryDopPostDay.Where(x => x.BrnCode == param.BrnCode);
            }
            if (param.DocDate != null && param.DocDate.HasValue)
            {
                qryDopPostDay = qryDopPostDay.Where(x => x.DocDate == param.DocDate.Value);
            }
            result.Result = await qryDopPostDay.ToArrayAsync();
            if (result.Result == null || 0.Equals(result.Result.Length))
            {
                result.IsSuccess = false;
                result.Message = "No Data Found";
            }
            else
            {
                result.IsSuccess = true;
                result.Message = "Load Data Complete";
            }
            return result;
        }

        public async Task<UpdateBillPaymentResult> UpdateBillPayment(UpdateBillPaymentParam param)
        {

            var result = new UpdateBillPaymentResult();
            try
            {

                if (param == null)
                {
                    result.Message = "Parameter Is Null";
                    return result;
                }
                if (param.ArrDocDate == null || 0.Equals(param.ArrDocDate.Length))
                {
                    result.Message = "Parameter DocDate Is Null Or Empty";
                    return result;
                }
                var qryPostDay = _context.DopPostdayHds.Where(
                    x => x.BrnCode == param.BrnCode
                    && x.CompCode == param.CompCode
                    && param.ArrDocDate.Contains(x.DocDate)
                );
                var arrPostDay = await qryPostDay.ToArrayAsync();
                if (arrPostDay == null || arrPostDay.Length == 0)
                {
                    result.Message = "No Row To Update";
                    return result;
                }
                foreach (var pd in arrPostDay)
                {
                    if (pd == null)
                    {
                        continue;
                    }
                    pd.BillPayment = param.IsUpdateBillPayment ? "Y" : "N";
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }

           
        }
    }
}
