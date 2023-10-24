using AutoMapper;
using MaxStation.Entities.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Resources;
using Sale.API.Resources.CashSale;
using Sale.API.Resources.CashTax;
using Sale.API.Resources.Quotation;

namespace Sale.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<SalCashsaleHd, CashSaleHdResource>();
            CreateMap<QueryResult<SalCashsaleHd>, QueryResultResource<CashSaleHdResource>>();

            CreateMap<SalQuotationHd, QuotationHdResource>();
            CreateMap<QueryResult<SalQuotationHd>, QueryResultResource<QuotationHdResource>>();

            CreateMap<SalTaxinvoiceHd, CashTaxHdResource>();
            CreateMap<QueryResult<SalTaxinvoiceHd>, QueryResultResource<CashTaxHdResource>>();
        }
    }
}
