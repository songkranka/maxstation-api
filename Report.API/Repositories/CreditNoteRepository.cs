using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Repositories
{
    public class CreditNoteRepository : SqlDataAccessHelper, ICreditNoteRepository
    {
        public CreditNoteRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<CreditNoteResponse.CreditNoteHd> GetDocumentAsync(CreditNoteRequest request)
        {
            var response = new CreditNoteResponse.CreditNoteHd();
            var query = (from hd in this.context.SalCndnHds
                         join br in this.context.MasBranches on new { hd.CompCode, hd.BrnCode } equals new { br.CompCode, br.BrnCode }
                         join cm in this.context.MasCompanies on hd.CompCode equals cm.CompCode
                         join tv in this.context.SalTaxinvoiceHds on hd.TxNo equals tv.DocNo
                         where hd.CompCode == request.CompCode && hd.BrnCode == request.BrnCode && hd.DocNo == request.DocNo
                         select new { hd, br, cm, tv }
                         ).AsQueryable();

            response = await query.Select(x => new CreditNoteResponse.CreditNoteHd
            {

                docNo = x.hd.DocNo,
                docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                txNo = x.hd.TxNo,
                compName = x.cm.CompName,
                compAddress = x.cm.Address,
                compPhone = x.cm.Phone,
                compFax = x.cm.Fax,
                compRegisterId = x.cm.RegisterId,
                compImage = x.cm.CompImage,
                brnCode = x.hd.BrnCode,
                brnName = x.br.BrnName,
                branchNo = x.br.BranchNo ?? "",
                brnAddress = x.br.FullAddress ?? "",
                reasonDesc = x.hd.ReasonDesc,
                custCode = x.hd.CustCode,
                custName = x.hd.CustName,
                custAddr1 = x.hd.CustAddr1 ?? "",
                custAddr2 = x.hd.CustAddr2 ?? "",
                subAmt = x.hd.SubAmt ?? 0,
                netAmt = x.hd.NetAmt ?? 0,
                vatAmt = x.hd.VatAmt ?? 0,
                docType = x.hd.DocType,
                citizenId = x.hd.CitizenId,
                taxInvoiceDocDate = x.tv.DocDate.Value.ToString("yyyy-MM-dd"),
            }).FirstOrDefaultAsync();

            if (response != null)
            {
                if (response.docType == "CreditNote")
                {
                    response.docType = "ใบลดหนี้";
                }
                else
                {
                    response.docType = "ใบเพิ่มหนี้";
                }

                var items = await this.context.SalCndnDts.Where(x => x.CompCode == request.CompCode && x.BrnCode == request.BrnCode && x.DocNo == request.DocNo).ToListAsync();

                response.items = items.Select(x => new CreditNoteResponse.CreditNoteDt
                {
                    seqNo = x.SeqNo,
                    pdId = x.PdId,
                    pdName = x.PdName,
                    beforeQty = x.BeforeQty,
                    afterQty = x.AfterQty,
                    beforePrice = x.BeforePrice,
                    afterPrice = x.AfterPrice,
                    beforeAmt = x.BeforeAmt,
                    afterAmt = x.AfterAmt,
                }).ToList();
                var employee = await this.context.MasEmployees.FirstOrDefaultAsync(x => x.EmpCode == request.EmpCode);
                response.empCode = (employee == null) ? "" : employee.EmpCode;
                response.empName = (employee == null) ? "" : employee.EmpName;
                response.netAmtText = response.netAmt == null ? "" : Function.ThaiBahtText(response.netAmt);
            }
            return response;
        }
    }
}
