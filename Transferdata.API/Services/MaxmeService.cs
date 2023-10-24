using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Models.Response;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;

namespace Transferdata.API.Services
{
    public class MaxmeService : IMaxmeService
    {
        private readonly IMaxmeRepository maxmeRepository;
        private readonly IUnitOfWork unitOfWork;
        private PTMaxstationContext Context;

        public MaxmeService(
            IMaxmeRepository _maxmeRepository,
            IUnitOfWork _unitOfWork,
            PTMaxstationContext _context)
        {
            maxmeRepository = _maxmeRepository;
            unitOfWork = _unitOfWork;
            Context = _context;            
        }

        public async Task<MaxmeResponse> GetMaxPosSum(MaxmeQuery query)
        {
            return await this.maxmeRepository.GetMaxPosSumAsync(query);
        }


    }
}
