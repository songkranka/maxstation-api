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
    public class ReceiveRepository : SqlDataAccessHelper, IReceiveRepository
    {
        public ReceiveRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<List<FinReceiveHd>> ListReceiveAsync(ReceiveQuery query)
        {
            List<FinReceiveHd> heads = new List<FinReceiveHd>();


            heads = await this.context.FinReceiveHds.Where(x => x.CompCode == query.CompCode
                                                       && x.BrnCode == query.BrnCode
                                                       && x.DocDate == query.DocDate                                                       
                                                        ).ToListAsync();
            heads.ForEach(hd => {
                    hd.FinReceiveDt = this.context.FinReceiveDts.Where(x => x.CompCode == hd.CompCode
                                                                                && x.BrnCode == hd.BrnCode
                                                                                && x.LocCode == hd.LocCode
                                                                                && x.DocNo == hd.DocNo).ToList();
                    hd.FinReceivePay = this.context.FinReceivePays.Where(x => x.CompCode == hd.CompCode
                                                                        && x.BrnCode == hd.BrnCode
                                                                        && x.LocCode == hd.LocCode
                                                                        && x.DocNo == hd.DocNo).ToList();
                } 
            );


            return heads;
        }
    }
}
