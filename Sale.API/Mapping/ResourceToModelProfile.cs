using AutoMapper;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Resources.CashSale;
using Sale.API.Resources.CashTax;
using Sale.API.Resources.Quotation;
using System;

namespace Sale.API.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<CashSaleHdQueryResource, CashSaleHdQuery>();
            CreateMap<SaveCashSaleResource, CashSale>();

            CreateMap<QuotationHdQueryResource, QuotationHdQuery>()
                .ConstructUsing(x => new QuotationHdQuery(x.CompCode, x.BrnCode, x.LocCode, x.DocNo, x.Keyword, x.Guid, x.PDListID, x.DocStatus, x.Page, x.ItemsPerPage));

            CreateMap<SaveCashTaxResource, CashTax>();
            CreateMap<CashTaxHdQueryResource, CashTaxHdQuery>()
                .ConstructUsing(x => new CashTaxHdQuery(x.BrnCode, x.CompCode, x.FromDate, x.ToDate, x.Keyword, x.Guid, x.Page, x.ItemsPerPage));
        }
    }
}
