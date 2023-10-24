using AutoMapper;
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

namespace Report.API.Repositories
{
    public class ReportAuditRepository : SqlDataAccessHelper, IReportAuditRepository
    {
        private readonly IMapper _mapper;

        public ReportAuditRepository(PTMaxstationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public ReportAuditDiffResponse GetAuditDiffPDF(string compCode, string brnCode, string docNo)
        {
            int seqNo = 0;
            var response = new ReportAuditDiffResponse();
            var brn = context.MasBranches.Where(x => x.CompCode == compCode && x.BrnCode == brnCode).FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == compCode);
            var auditHeader = context.InvAuditHds.FirstOrDefault(x => x.DocNo == docNo);
            var auditDetails = (from ad in context.InvAuditDts
                                join pg in context.MasProducts on ad.PdId equals pg.PdId
                                where ad.CompCode == compCode && ad.BrnCode == brnCode && ad.DocNo == docNo && ad.DiffQty != 0
                                select new
                                {
                                    ad.CompCode,
                                    ad.BrnCode,
                                    ad.LocCode,
                                    ad.DocNo,
                                    ad.SeqNo,
                                    ad.PdId,
                                    ad.PdName,
                                    pg.GroupId,
                                    ad.UnitId,
                                    ad.UnitBarcode,
                                    ad.UnitName,
                                    ad.UnitPrice,
                                    ad.UnitCost,
                                    ad.BalanceQty,
                                    ad.ItemQty,
                                    ad.DiffQty,
                                    ad.AdjustQty,
                                    ad.NoadjQty,
                                }).ToList();

            var auditResponses = new List<ReportAuditDiffResponse.Audit>();

            if (auditDetails != null && auditDetails.Count() > 0)
            {
                var productGroupIds = auditDetails.OrderBy(x => x.GroupId).Select(p => p.GroupId).Distinct();
                var masProductGroups = context.MasProductGroups.Where(x => productGroupIds.Contains(x.GroupId)).ToList();
                var productIds = auditDetails.Select(x => x.PdId).ToList();
                var masProducts = context.MasProducts.Where(x => productIds.Contains(x.PdId)).ToList();
                var masProductPrices = context.MasProductPrices.Where(x =>x.CompCode == compCode && x.BrnCode == brnCode && productIds.Contains(x.PdId)).ToList();

                foreach (var productGroupId in productGroupIds)
                {
                    var auditResponse = new ReportAuditDiffResponse.Audit();
                    var productGroupName = masProductGroups.FirstOrDefault(x => x.GroupId == productGroupId).GroupName;
                    var products = masProducts.Where(x => x.GroupId == productGroupId).OrderBy(x => x.PdId).ToList();

                    auditResponse.productGroupId = productGroupId;
                    auditResponse.productGroupName = productGroupName;
                    auditResponse.auditDts = new List<ReportAuditDiffResponse.AuditDt>();
                   // var auditDetailsResponse = new List<ReportAuditDiffResponse.AuditDt>();
                    var sumBanlanceQty = 0m;
                    var sumItemQty = 0m;
                    var sumDiffQty = 0m;
                    var sumAdjustQty = 0m;
                    var sumNoAdjustQty = 0m;
                    var sumTotal = 0m;

                    foreach (var product in products)
                    {
                        var auditDetail = new ReportAuditDiffResponse.AuditDt();
                        var masProductPrice = masProductPrices.FirstOrDefault(x => x.PdId == product.PdId).Unitprice ?? 0;
                        var audit = auditDetails.FirstOrDefault(x => x.PdId.Trim() == product.PdId.Trim());
                        var banlanceQty = audit.BalanceQty ?? 0;
                        var itemQty = audit.ItemQty ?? 0;
                        var diffQty = audit.DiffQty ?? 0;
                        var adjustQty = audit.AdjustQty ?? 0;
                        var noAdjustQty = audit.NoadjQty ?? 0;
                        var total = masProductPrice * audit.AdjustQty ?? 0;

                        sumBanlanceQty += banlanceQty;
                        sumItemQty += itemQty;
                        sumDiffQty += diffQty;
                        sumAdjustQty += adjustQty;
                        sumNoAdjustQty += noAdjustQty;
                        sumTotal += total;

                        auditDetail.seqNo = ++seqNo;
                        auditDetail.productId = product.PdId;
                        auditDetail.productName = product.PdName;
                        auditDetail.productPrice = masProductPrice;
                        auditDetail.productGroupId = productGroupId;
                        auditDetail.productGroupName = productGroupName;
                        
                        auditDetail.balanceQty = banlanceQty;
                        auditDetail.sumBanlanceQty = sumBanlanceQty;
                        auditDetail.itemQty = itemQty;
                        auditDetail.sumItemQty = sumItemQty;
                        auditDetail.diffQty = diffQty;
                        auditDetail.sumDiffQty = sumDiffQty;
                        auditDetail.adjustQty = adjustQty;
                        auditDetail.sumAdjustQty = sumAdjustQty;
                        auditDetail.noAdjustQty = noAdjustQty;
                        auditDetail.sumNoAdjustQty = sumNoAdjustQty;
                        auditDetail.total = total;
                        auditDetail.sumTotal = sumTotal;

                        //auditDetailsResponse.Add(auditDetail);
                        //auditResponse.auditDts = auditDetailsResponse;
                        auditResponse.auditDts.Add(auditDetail);
                    }

                    auditResponses.Add(auditResponse);
                }
            }

            response.brnCode = brn.BrnCode;
            response.brnName = brn.BrnName;
            response.compName = company.CompName;
            response.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png"; // company.CompImage;
            response.docNo = auditHeader.DocNo;
            var docDate = auditHeader.DocDate ?? DateTime.Now;
            response.docDate = docDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            response.audits = auditResponses;
            response.balanceQty = auditResponses.Sum(x => x.auditDts.Sum(s => s.balanceQty));
            response.itemQty = auditResponses.Sum(x => x.auditDts.Sum(s => s.itemQty));
            response.diffQty = auditResponses.Sum(x => x.auditDts.Sum(s => s.diffQty));
            response.adjustQty = auditResponses.Sum(x => x.auditDts.Sum(s => s.adjustQty));
            response.sumTotal = auditResponses.Sum(x => x.auditDts.Sum(s => s.total));
            response.noAdjustQty = auditResponses.Sum(x => x.auditDts.Sum(s => s.noAdjustQty));

            return response;
        }

        public ReportAuditDetailResponse GetAuditDetailPDF(string compCode, string brnCode, string docNo)
        {
            int seqNo =0;
            var response = new ReportAuditDetailResponse();
            var brn = context.MasBranches.Where(x => x.CompCode == compCode && x.BrnCode == brnCode).FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == compCode);

            var auditHeader = context.InvAuditHds.FirstOrDefault(x =>x.CompCode == compCode && x.BrnCode == brnCode && x.DocNo == docNo);
            var auditDetails = (from ad in context.InvAuditDts
                                join pg in context.MasProducts on ad.PdId equals pg.PdId
                                where ad.CompCode == compCode && ad.BrnCode == brnCode && ad.DocNo == docNo
                                select new
                                {
                                    ad.BrnCode,
                                    ad.LocCode,
                                    ad.DocNo,
                                    ad.SeqNo,
                                    ad.PdId,
                                    ad.PdName,
                                    pg.GroupId,
                                    ad.UnitId,
                                    ad.UnitBarcode,
                                    ad.UnitName,
                                    ad.UnitPrice,
                                    ad.UnitCost,
                                    ad.BalanceQty,
                                    ad.ItemQty,
                                    ad.DiffQty,
                                    ad.AdjustQty,
                                    ad.NoadjQty,
                                }).ToList();

            var auditResponses = new List<ReportAuditDetailResponse.Audit>();

            if (auditDetails != null && auditDetails.Count() > 0)
            {
                var productGroupIds = auditDetails.OrderBy(x => x.GroupId).Select(p => p.GroupId).Distinct();
                var masProductGroups = context.MasProductGroups.Where(x => productGroupIds.Contains(x.GroupId)).ToList();
                var productIds = auditDetails.Select(x => x.PdId).ToList();
                var masProducts = context.MasProducts.Where(x => productIds.Contains(x.PdId)).ToList();
                var masProductPrices = context.MasProductPrices.Where(x =>x.CompCode == compCode && x.BrnCode == brnCode && productIds.Contains(x.PdId)).ToList();

                foreach (var productGroupId in productGroupIds)
                {
                    var auditResponse = new ReportAuditDetailResponse.Audit();
                    var productGroupName = masProductGroups.FirstOrDefault(x => x.GroupId == productGroupId).GroupName;
                    var products = masProducts.Where(x => x.GroupId == productGroupId).OrderBy(x => x.PdId).ToList();

                    auditResponse.productGroupId = productGroupId;
                    auditResponse.productGroupName = productGroupName;
                    auditResponse.auditDts = new List<ReportAuditDetailResponse.AuditDt>();
                    //var auditDetailsResponse = new List<ReportAuditDetailResponse.AuditDt>();
                    var sumBanlanceQty = 0m;
                    var sumItemQty = 0m;
                    var sumDiffQty = 0m;
                    var sumAdjustQty = 0m;
                    var sumNoAdjustQty = 0m;
                    var sumTotal = 0m;

                    foreach (var product in products)
                    {
                        var auditDetail = new ReportAuditDetailResponse.AuditDt();
                        var masProductPrice = masProductPrices.FirstOrDefault(x => x.PdId == product.PdId).Unitprice ?? 0;
                        var audit = auditDetails.FirstOrDefault(x => x.PdId.Trim() == product.PdId.Trim());
                        var banlanceQty = audit.BalanceQty ?? 0;
                        var itemQty = audit.ItemQty ?? 0;
                        var diffQty = audit.DiffQty ?? 0;
                        var adjustQty = audit.AdjustQty ?? 0;
                        var noAdjustQty = audit.NoadjQty ?? 0;
                        var total = masProductPrice * audit.AdjustQty ?? 0;

                        sumBanlanceQty += banlanceQty;
                        sumItemQty += itemQty;
                        sumDiffQty += diffQty;
                        sumAdjustQty += adjustQty;
                        sumNoAdjustQty += noAdjustQty;
                        sumTotal += total;

                        auditDetail.seqNo = ++seqNo;
                        auditDetail.productId = product.PdId;
                        auditDetail.productName = product.PdName;
                        auditDetail.productPrice = masProductPrice;
                        auditDetail.productGroupId = productGroupId;
                        auditDetail.productGroupName = productGroupName;

                        auditDetail.balanceQty = banlanceQty;
                        auditDetail.sumBanlanceQty = sumBanlanceQty;
                        auditDetail.itemQty = itemQty;
                        auditDetail.sumItemQty = sumItemQty;
                        auditDetail.diffQty = diffQty;
                        auditDetail.sumDiffQty = sumDiffQty;
                        auditDetail.adjustQty = adjustQty;
                        auditDetail.sumAdjustQty = sumAdjustQty;
                        auditDetail.noAdjustQty = noAdjustQty;
                        auditDetail.sumNoAdjustQty = sumNoAdjustQty;
                        auditDetail.total = total;
                        auditDetail.sumTotal = sumTotal;

                        //auditDetailsResponse.Add(auditDetail);
                        //auditResponse.auditDts = auditDetailsResponse;
                        auditResponse.auditDts.Add(auditDetail);
                    }

                    auditResponses.Add(auditResponse);
                }
            }

            response.brnCode = brn.BrnCode;
            response.brnName = brn.BrnName;
            response.compName = company.CompName;
            response.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png"; // company.CompImage;
            response.docNo = auditHeader.DocNo;            
            response.docDate = auditHeader.DocDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            response.audits = auditResponses;
            response.balanceQty = auditResponses.Sum(x => x.auditDts.Sum(s => s.balanceQty));
            response.itemQty = auditResponses.Sum(x => x.auditDts.Sum(s => s.itemQty));
            response.diffQty = auditResponses.Sum(x => x.auditDts.Sum(s => s.diffQty));
            response.adjustQty = auditResponses.Sum(x => x.auditDts.Sum(s => s.adjustQty));
            response.sumTotal = auditResponses.Sum(x => x.auditDts.Sum(s => s.total));
            response.noAdjustQty = auditResponses.Sum(x => x.auditDts.Sum(s => s.noAdjustQty));
            return response;
        }
    }
}
