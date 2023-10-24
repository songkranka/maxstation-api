using Common.API.Resource.Auth;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Domain.Response
{
    public class AuthenticatedResponse
    {
        public string Token { get; set; }
        public List<BranchResource> Branches { get; set; }
        public List<AutPositionRole> PositionRoles { get; set; }
    }
}
