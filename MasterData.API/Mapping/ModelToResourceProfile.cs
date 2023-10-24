using AutoMapper;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Resources;
using MasterData.API.Resources.Branch;
using MasterData.API.Resources.Customer;
using MasterData.API.Resources.Product;
using MaxStation.Entities.Models;

namespace MasterData.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<QueryResult<MasCustomer>, QueryResultResource<CustomerResource>>();
            CreateMap<QueryResult<MeterResponse>, QueryResultResource<MeterResponse>>();
            CreateMap<MasBranch, BranchResource>();
            CreateMap<MasCustomerCar, CustomerCompanyCar>();
        }
    }
}
