using MaxStation.Entities.Models;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Report.API.Domain.Models.Response.WithdrawResponse;

namespace Report.API.Repositories
{
    public class WithdrawRepository : SqlDataAccessHelper, IWithdrawRepository
    {
        public WithdrawRepository(PTMaxstationContext context) : base(context)
        {
        }

        public WithdrawHd GetDocumentPDF(WithdrawRequest.GetDocumentRequest req)
        {
            WithdrawHd response = new WithdrawHd();

            var branch = (from b in this.context.MasBranches
                          join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode
                          && b.BrnCode == req.BrnCode                          
                          select new { b, c }).FirstOrDefault();

            response.compCode = branch.c.CompCode;
            response.compName = branch.c.CompName;
            response.compNameEn = branch.c.CompNameEn ?? "";
            response.compAddress = branch.c.Address ?? "";
            response.compPhone = branch.c.Phone ?? "";
            response.compFax = branch.c.Fax ?? "";
            response.registerId = branch.c.RegisterId;
            response.compImage = branch.c.CompImage;
            response.brnCode = branch.b.BrnCode;
            response.brnName = branch.b.BrnName;
            response.brnAddress = branch.b.Address;

            var head = this.context.InvWithdrawHds.FirstOrDefault(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocNo == req.docNo);
            response.docNo = head.DocNo;            
            response.docDate = head.DocDate.Value.ToString("yyyy-MM-dd");
            response.empCode = head.EmpCode;
            response.empName = head.EmpName ?? "";
            response.licensePlate = head.LicensePlate;
            response.useBrnCode = head.UseBrnCode;
            response.useBrnName = head.UseBrnName;
            response.reasonDesc = head.ReasonDesc ?? "";
            response.remark = head.Remark ?? "";

            var query = this.context.InvWithdrawDts.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocNo == req.docNo).AsQueryable();
            response.items = query.Select(x => new WithdrawDt
                                {
                                    seqNo = x.SeqNo,
                                    pdId = x.PdId,
                                    pdName = x.PdName,
                                    unitName = x.UnitName,
                                    itemQty = x.ItemQty??0
                                }).OrderBy(x=>x.seqNo).ToList();
            response.totalQty = response.items.Sum(x => x.itemQty);
            response.itemCount = response.items.Count;
            return response;
        }
    }
}
