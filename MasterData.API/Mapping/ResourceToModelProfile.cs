using AutoMapper;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Resources;
using MasterData.API.Resources.Branch;
using MasterData.API.Resources.CompanyCar;
using MasterData.API.Resources.CostCenter;
using MasterData.API.Resources.Customer;
using MasterData.API.Resources.CustomerCar;
using MasterData.API.Resources.Employee;
using MasterData.API.Resources.EmployeeAuth;
using MasterData.API.Resources.Product;
using MasterData.API.Resources.ProductGroup;
using MasterData.API.Resources.Reason;
using MasterData.API.Resources.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<CustomerQueryResource, CustomerQuery>();
            CreateMap<CompanyCarQueryReqource, CompanyCarQuery>();
            CreateMap<CustomerCarQueryResource, Domain.Models.Queries.CustomerCarQuery>();
            CreateMap<EmployeeQueryResource, EmployeeQuery>();
            CreateMap<CosCenterQueryResource, CosCenterQuery>();
            CreateMap<BranchQueryReqource, BranchQuery>();
            CreateMap<WithdrawQueryResource, WithdrawQuery>();
            CreateMap<ProductQueryResource, ProductQuery>();
            CreateMap<ProductGroupQueryResource, ProductGroupQuery>();
            CreateMap<EmployeeAuthQueryReqource, EmployeeAuthQuery>();
            CreateMap<MasCosCenterQueryResource, MasCosCenterQuery>();
            CreateMap<UnitQueryResource, UnitQuery>();
            CreateMap<ProductReasonQueryResource, ProductReasonQuery>();
        }
    }
}
