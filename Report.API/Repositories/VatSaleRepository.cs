using AutoMapper;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Repositories
{
    public class VatSaleRepository : SqlDataAccessHelper, IVatSaleRepository
    {
        private readonly IMapper _mapper;
        public VatSaleRepository(MaxStation.Entities.Models.PTMaxstationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public VatSaleResponse GetVatSalePDF(VatSaleRequest req)
        {
            VatSaleResponse result = new VatSaleResponse();
            //var branch = this.context.MasBranches.FirstOrDefault(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode);

            var query = (from b in this.context.MasBranches
                        join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                        where b.CompCode == req.CompCode &&
                        b.BrnCode == req.BrnCode
                        select new { b, c }).FirstOrDefault();

            result.brnCode = query.b.BrnCode;
            result.brnName = query.b.BrnName;
            result.branchNo = query.b.BranchNo;
            result.brnAddress = query.b.FullAddress;
            result.companyName = query.c.CompName;
            result.registerId = query.c.RegisterId;

            return result;
        }


    }
}
