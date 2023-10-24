using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
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
    public class TaxInvoiceRepository : SqlDataAccessHelper, ITaxInvoiceRepository
    {
        public TaxInvoiceRepository(PTMaxstationContext context) : base(context)
        {

        }
        public async Task<TaxInvoice> FindByIdAsync(string guid, string printby)
        {
            var response = new TaxInvoice();

            var salTaxInvoiceHd = await context.SalTaxinvoiceHds.AsNoTracking().FirstOrDefaultAsync(x => x.Guid.Value.ToString().Contains(guid));

            if (salTaxInvoiceHd != null)
            {
                var salTaxInvoiceDt = context.SalTaxinvoiceDts.Where(x => x.CompCode == salTaxInvoiceHd.CompCode
                                                                                 && x.BrnCode == salTaxInvoiceHd.BrnCode
                                                                                 && x.LocCode == salTaxInvoiceHd.LocCode
                                                                                 && x.DocNo == salTaxInvoiceHd.DocNo).ToList();
                var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == salTaxInvoiceHd.CompCode);
                var companyCustomer = context.MasCustomers.FirstOrDefault(x => x.CustCode == company.CustCode);
                var customer = context.MasCustomers.FirstOrDefault(x => x.CustCode == salTaxInvoiceHd.CustCode);
                var branch = context.MasBranches.FirstOrDefault(x => x.BrnCode == salTaxInvoiceHd.BrnCode);

                response.SalTaxinvoiceHd = salTaxInvoiceHd;
                response.SalTaxinvoiceDt = salTaxInvoiceDt.Select(x => new TaxInvoice.TaxInvoiceDt
                {
                    ItemQty = x.ItemQty,
                    Product = x.PdId + "-" + x.PdName,
                    UnitName = x.UnitName,
                    UnitPrice = x.UnitPrice,
                    Discount = x.DiscHdAmtCur,
                    SubAmtCur = x.SubAmtCur
                }).ToList();
                response.MasCompany = company;
                response.MasCompanyCustomer = companyCustomer;
                response.MasBranch = branch;
                response.MasCustomer = customer;
            }

            return response;
        }

        public async Task<List<TaxInvoice>> GetTaxInvoiceListAsync(string guid, string printby)
        {
            var response = new List<TaxInvoice>();

            var salTaxInvoiceHds = await context.SalTaxinvoiceHds.AsNoTracking().Where(x => x.Guid.Value.ToString().Contains(guid)).ToListAsync();

            if (salTaxInvoiceHds != null)
            {
                foreach (var salTaxInvoiceHd in salTaxInvoiceHds)
                {
                    var salTaxInvoice = new TaxInvoice();
                    var salTaxInvoiceDt = context.SalTaxinvoiceDts.Where(x => x.CompCode == salTaxInvoiceHd.CompCode
                                                                                 && x.BrnCode == salTaxInvoiceHd.BrnCode
                                                                                 && x.LocCode == salTaxInvoiceHd.LocCode
                                                                                 && x.DocNo == salTaxInvoiceHd.DocNo).ToList();
                    var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == salTaxInvoiceHd.CompCode);
                    var companyCustomer = context.MasCustomers.FirstOrDefault(x => x.CustCode == company.CustCode);
                    var customer = context.MasCustomers.FirstOrDefault(x => x.CustCode == salTaxInvoiceHd.CustCode);
                    var branch = context.MasBranches.FirstOrDefault(x => x.BrnCode == salTaxInvoiceHd.BrnCode);
                    salTaxInvoice.SalTaxinvoiceHd = salTaxInvoiceHd;
                    salTaxInvoice.MasCompany = company;
                    salTaxInvoice.MasCompanyCustomer = companyCustomer;
                    salTaxInvoice.MasBranch = branch;
                    salTaxInvoice.MasCustomer = customer;
                    salTaxInvoice.SalTaxinvoiceDt = salTaxInvoiceDt.Select(x => new TaxInvoice.TaxInvoiceDt
                    {
                        ItemQty = x.ItemQty,
                        Product = x.PdId + "-" + x.PdName,
                        UnitName = x.UnitName,
                        UnitPrice = x.UnitPrice,
                        Discount = x.DiscHdAmtCur,
                        SubAmtCur = x.SubAmtCur
                    }).ToList();

                    response.Add(salTaxInvoice);
                };
            }

            return response;
        }

        public async Task<TaxInvoiceHd> GetTaxInvoiceAsync(string guid, string empcode)
        {
            var header = new TaxInvoiceHd();
            Guid guId = new Guid(guid);

            var query = (from head in this.context.SalTaxinvoiceHds
                         join comp in this.context.MasCompanies on head.CompCode equals comp.CompCode
                         join brn in this.context.MasBranches on head.BrnCode equals brn.BrnCode
                         //into merge
                         //from right in merge.DefaultIfEmpty()
                         where head.Guid == guId
                         select new { head, comp, brn }
                         ).AsQueryable();

            header = await query.Select(x => new TaxInvoiceHd
            {
                comp_code = x.head.CompCode,
                company_name = (x.comp == null) ? "" : x.comp.CompName,
                company_address = (x.comp == null) ? "" : x.comp.Address,
                company_phone = (x.comp == null) ? "" : x.comp.Phone,
                company_fax = (x.comp == null) ? "" : x.comp.Fax,
                company_image = (x.comp == null) ? "" : x.comp.CompImage,
                company_cust_code = (x.comp == null) ? "" : x.comp.CustCode,
                brn_code = x.head.BrnCode,
                brn_name = (x.brn == null) ? "" : x.brn.BrnName,
                brn_address = (x.brn == null) ? "" : x.brn.Address + " " + x.brn.Postcode,
                brn_no = (x.brn == null) ? "" : x.brn.BranchNo,
                loc_code = x.head.LocCode,
                doc_no = x.head.DocNo,
                doc_date = string.Format("{0:dd/MM/yyyy}", x.head.DocDate),
                cust_code = x.head.CustCode,
                cust_name = x.head.CustName,
                cust_addr1 = x.head.CustAddr1,
                cust_addr2 = x.head.CustAddr2,
                citizen_id = x.head.CitizenId,
                sub_amt = x.head.SubAmt,
                disc_amt = x.head.DiscAmt,
                vat_amt = x.head.VatAmt,
                net_amt = x.head.NetAmt,
                tax_base_amt = x.head.TaxBaseAmt
            }).FirstOrDefaultAsync();

            header.net_amt_text = header.net_amt == null ? "" : Function.ThaiBahtText(header.net_amt);

            if (header != null)
            {
                header.items = await context.SalTaxinvoiceDts.Where(x => x.CompCode == header.comp_code
                                                                        && x.BrnCode == header.brn_code
                                                                        && x.LocCode == header.loc_code
                                                                        && x.DocNo == header.doc_no)
                                                                     .Select(x => new TaxInvoiceDt
                                                                     {
                                                                         item_qty = x.ItemQty,
                                                                         product_name = x.PdId + "-" + x.PdName,
                                                                         unit_name = x.UnitName,
                                                                         unit_price = x.UnitPrice,
                                                                         disc_amt = x.DiscAmt,
                                                                         sub_amt = x.SubAmt,
                                                                         seq_no = x.SeqNo,
                                                                         license_plate = x.LicensePlate
                                                                     }).ToListAsync();

                var company_customer = await context.MasCustomers.FirstOrDefaultAsync(x => x.CustCode == header.company_cust_code);
                if (company_customer != null)
                    header.company_citizenid = company_customer.CitizenId;

                var employee = await context.MasEmployees.FirstOrDefaultAsync(x => x.EmpCode == empcode);
                if (employee != null)
                    header.emp_name = employee.PersonFnameThai + " " + employee.PersonLnameThai;

                var license_plate = string.Join(", ", header.items.Select(x => x.license_plate));
                header.license_plate = (license_plate == ", ") ? "-" : license_plate;

                #region Update Print
                var print = this.context.SalTaxinvoiceHds.FirstOrDefault(x => x.CompCode == header.comp_code
                                                            && x.BrnCode == header.brn_code
                                                            && x.LocCode == header.loc_code
                                                            && x.DocNo == header.doc_no);

                print.PrintCount = (print.PrintCount ?? 0) + 1;
                print.PrintBy = empcode;
                print.PrintDate = DateTime.Now;
                #endregion
            }

            return header;
        }

        public void UpdateSalTaxInvoiceAsync(SalTaxinvoiceHd salTaxinvoiceHd)
        {
            context.SalTaxinvoiceHds.Update(salTaxinvoiceHd);
        }

        public async Task<TaxInvoiceResponse> GetTaxInvoice2Async(TaxInvoiceRequest request)
        {
            TaxInvoiceResponse response = new TaxInvoiceResponse();

            var query = (from hd in this.context.SalTaxinvoiceHds                                                  
                         join br in this.context.MasBranches on new {hd.CompCode,hd.BrnCode } equals new { br.CompCode,br.BrnCode}                         
                         join cm in this.context.MasCompanies on hd.CompCode equals cm.CompCode
                         where hd.CompCode == request.CompCode
                         && hd.BrnCode == request.BrnCode
                         && hd.DocNo == request.DocNo
                         select new { hd,br,cm }
                         ).AsQueryable();

            response = await query.Select(x => new TaxInvoiceResponse
                                    {
                                        docType = x.hd.DocType,
                                        docNo = x.hd.DocNo,
                                        docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                                        compCode = x.cm.CompCode,
                                        compName = x.cm.CompName,
                                        compAddress = x.cm.Address??"",
                                        compPhone = x.cm.Phone??"",
                                        compFax = x.cm.Fax??"",
                                        compRegisterId = x.cm.RegisterId??"",
                                        compImage = x.cm.CompImage,
                                        brnCode = x.hd.BrnCode,
                                        brnName = x.br.BrnName,
                                        branchNo = x.br.BranchNo??"",
                                        brnAddress = x.br.FullAddress??"",                                        
                                        custCode = x.hd.CustCode,
                                        custName = x.hd.CustName,
                                        custAddress1 = x.hd.CustAddr1??"",
                                        custAddress2 = x.hd.CustAddr2??"",
                                        custCitizenId = x.hd.CitizenId??"",
                                        refDocNo = x.hd.RefDocNo??"",
                                        taxBaseAmt = x.hd.TaxBaseAmt??0,
                                        taxBaseAmtHd = x.hd.TaxBaseAmt??0,
                                        vatAmt = x.hd.VatAmt??0,
                                        subAmt = x.hd.SubAmt??0,
                                        netAmt = x.hd.NetAmt??0
                                    }).FirstOrDefaultAsync();
            if (response != null)
            {
                response.items = await this.context.SalTaxinvoiceDts.Where(x => x.CompCode == request.CompCode && x.BrnCode == request.BrnCode && x.DocNo == request.DocNo)
                                                    .Select(x => new TaxInvoiceItem
                                                    {
                                                        seqNo = x.SeqNo,
                                                        licensePlate = x.LicensePlate,
                                                        pdId = x.PdId,
                                                        pdName = x.PdName,
                                                        unitName = x.UnitName ?? "",
                                                        itemQty = x.ItemQty ?? 0,
                                                        unitPrice = x.UnitPrice ?? 0,
                                                        discAmt = x.DiscAmt ?? 0,
                                                        subAmt = x.SubAmt ?? 0
                                                    }).ToListAsync();
                response.netAmtText = Function.ThaiBahtText(response.netAmt);
                response.licensePlate = String.Join(",", response.items.Select(x => x.licensePlate).ToList());

                var vattypes = context.SalTaxinvoiceDts.Where(x => x.CompCode == request.CompCode && x.BrnCode == request.BrnCode && x.DocNo == request.DocNo).Select(x=>x.VatType).ToList();

                if (vattypes.Contains("NV") && vattypes.Contains("VI"))
                {

                    response.taxBaseAmt = (decimal)context.SalTaxinvoiceDts.Where(x => x.DocNo == request.DocNo && x.VatType != "NV").AsEnumerable().Sum(o => o.TaxBaseAmt);
                }
                else
                {
                    response.taxBaseAmt = (decimal)context.SalTaxinvoiceDts.Where(x => x.DocNo == request.DocNo).AsEnumerable().Sum(o => o.TaxBaseAmt);
                }

                var employee = await this.context.MasEmployees.FirstOrDefaultAsync(x => x.EmpCode == request.EmpCode);
                var customer = await this.context.MasCustomers.FirstOrDefaultAsync(x => x.CustCode == response.custCode);

                response.empName = (employee == null) ? "" : employee.EmpName;
                response.sapCustCode = (customer == null) ? "" : customer.MapCustCode;
                response.ref2 = "4"; //(sapcustomer == null) ? "" : sapcustomer.Ref2;
            }


            return response;
        }
    }
}
