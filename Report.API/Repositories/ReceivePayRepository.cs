using AutoMapper;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Repositories
{
    public class ReceivePayRepository : SqlDataAccessHelper, IReceivePayRepository
    {
        private readonly IMapper _mapper;
        public ReceivePayRepository(MaxStation.Entities.Models.PTMaxstationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public MemoryStream GetReceivePayExcel(ReportStockRequest req)
        {
            throw new NotImplementedException();
        }

        public async Task<ReceivePayResponse> GetReceivePayPDF(ReceivePayRequest req)
        {
            var response = new ReceivePayResponse();

            var finreceiveHd = await this.context.FinReceiveHds.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocNo == req.DocNo).FirstOrDefaultAsync();
            var branch = await this.context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).FirstOrDefaultAsync();
            var customer = await this.context.MasCustomers.Where(x => x.CustCode == finreceiveHd.CustCode).FirstOrDefaultAsync();
            var company = await this.context.MasCompanies.Where(x => x.CompCode == req.CompCode).FirstOrDefaultAsync();

            switch (finreceiveHd.ReceiveTypeId)
            {
                case "1": response.headerName = "ใบเสร็จรับเงิน";break; //1 รับชำระ
                case "2": response.headerName = "ใบเสร็จรับเงิน";break;    //2 รับล่วงหน้า
                case "3": response.headerName = "ใบกำกับภาษี/ใบเสร็จ";break; //3 รับอื่นๆ
            }
            response.compName = company.CompName;
            response.compAddress = company.Address;
            response.compPhone = company.Phone;
            response.compFax = company.Fax;
            response.compImage = company.CompImage;
            response.brnCode = branch.BranchNo;
            response.brnName = branch.BrnName;
            response.brnAddress = branch.FullAddress;
            response.docNo = req.DocNo;
            response.docDate = finreceiveHd.DocDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            response.custCode = finreceiveHd.CustCode;
            response.custName = finreceiveHd.CustName;
            response.custAddr1 = finreceiveHd.CustAddr1;
            response.custAddr2 = finreceiveHd.CustAddr2;
            response.citizenId = customer.CitizenId ?? "";
            response.receiveTypeId = finreceiveHd.ReceiveTypeId;
            response.receiveType = finreceiveHd.ReceiveType;
            response.payTypeId = finreceiveHd.PayTypeId;
            response.payType = finreceiveHd.PayType;
            response.payDate = (finreceiveHd.PayDate == null) ? response.docDate : finreceiveHd.PayDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            response.remark = finreceiveHd.Remark??"";

            response.totalAmt = finreceiveHd.TotalAmt??0;
            response.vatAmt = finreceiveHd.VatAmt??0;
            response.netAmt = finreceiveHd.NetAmt ?? 0;
            
            response.netAmtText = Function.ThaiBahtText(finreceiveHd.NetAmt ?? 0);

            var finReceiveDts = await this.context.FinReceiveDts.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocNo == req.DocNo)
                                            .Select(x => new finReceivePayDt
                                            {
                                                pdId = x.PdId,
                                                pdName = x.PdName,
                                                itemAmt = x.ItemAmt ?? 0
                                            }).ToListAsync();

            response.finReceivePayDt = new List<finReceivePayDt>(finReceiveDts);
            if (finreceiveHd.WhtAmt > 0)
            {
                finReceivePayDt wht = new finReceivePayDt();
                wht.pdName = "ภาษีถูกหัก ณ ที่จ่าย";
                wht.itemAmt = finreceiveHd.WhtAmt??0;
                response.finReceivePayDt.Add(wht);
            }

            return response;
        }
    }
}
