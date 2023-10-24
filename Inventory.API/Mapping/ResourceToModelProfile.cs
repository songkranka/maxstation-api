using AutoMapper;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Resources.ReceiveOil;
using Inventory.API.Resources.Withdraw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<WithdrawQueryResource, WithdrawQuery>();
            //.ConstructUsing(x => new WithdrawQuery(x.BrnCode, x.CompCode, x.FromDate, x.ToDate, x.Keyword, x.Page, x.ItemsPerPage));
            //CreateMap<SaveCashSaleResource, CashSale>();

            //CreateMap<QuotationHdQueryResource, QuotationHdQuery>()
            //    .ConstructUsing(x => new QuotationHdQuery(x.CompCode, x.BrnCode, x.LocCode, x.DocNo, x.Keyword, x.Guid, x.PDListID, x.DocStatus, x.Page, x.ItemsPerPage));

            //CreateMap<SaveCashTaxResource, CashTax>();
            CreateMap<ReceiveOilHdQueryResource, ReceiveOilHdQuery>()
                .ConstructUsing(x => new ReceiveOilHdQuery(x.BrnCode, x.CompCode, x.LocCode, x.FromDate, x.ToDate, x.Keyword, x.Guid, x.Page, x.ItemsPerPage));
        }
    }
}
