using AutoMapper;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Resources;
using Inventory.API.Resources.Adjust;
using Inventory.API.Resources.AdjustRequest;
using Inventory.API.Resources.Withdraw;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<InvWithdrawHd, WithdrawResource>();
            CreateMap<QueryResult<InvWithdrawHd>, QueryResultResource<WithdrawResource>>();

            CreateMap<InvAdjustRequestHd, AdjustRequestResource>();
            CreateMap<QueryResult<InvAdjustRequestHd>, QueryResultResource<AdjustRequestResource>>();

            CreateMap<InvAdjustHd, AdjustResource>();
            CreateMap<QueryResult<InvAdjustHd>, QueryResultResource<AdjustResource>>();

            CreateMap<InvReceiveProdDt, MasProduct>().ReverseMap();

            //CreateMap<SalQuotationHd, QuotationHdResource>();
            //CreateMap<QueryResult<SalQuotationHd>, QueryResultResource<QuotationHdResource>>();

            //CreateMap<SalTaxinvoiceHd, CashTaxHdResource>();
            //CreateMap<QueryResult<SalTaxinvoiceHd>, QueryResultResource<CashTaxHdResource>>();
        }
    }
}
