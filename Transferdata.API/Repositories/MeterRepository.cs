using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Helpers;

namespace Transferdata.API.Repositories
{
    public class MeterRepository : SqlDataAccessHelper, IMeterRepository
    {
        public MeterRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<DopPeriod> GetPeriodAsync(PeriodQuery query)
        {
            DopPeriod period = new DopPeriod();
            period = await this.context.DopPeriods.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.DocDate && x.PeriodNo == query.PeriodNo).FirstOrDefaultAsync();
            period.DopPeriodEmp = await this.context.DopPeriodEmps.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.DocDate && x.PeriodNo == query.PeriodNo).ToListAsync();
            period.DopPeriodMeter = await this.context.DopPeriodMeters.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.DocDate && x.PeriodNo == query.PeriodNo).ToListAsync();
            period.DopPeriodTank = await this.context.DopPeriodTanks.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.DocDate && x.PeriodNo == query.PeriodNo).ToListAsync();
            period.DopPeriodTankSum = await this.context.DopPeriodTankSums.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.DocDate && x.PeriodNo == query.PeriodNo).ToListAsync();
            period.DopPeriodCash = await this.context.DopPeriodCashes.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.DocDate && x.PeriodNo == query.PeriodNo).ToListAsync();
            period.DopPeriodCashSum = await this.context.DopPeriodCashSums.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.DocDate && x.PeriodNo == query.PeriodNo).FirstOrDefaultAsync();

            return period;
        }
    }
}
