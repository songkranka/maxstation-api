using AutoMapper;
using Common.API.Resource.Auth;
using MaxStation.Entities.Models;

namespace Common.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<MasBranch, BranchResource>();
        }
        
    }
}
