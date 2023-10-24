using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Response;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;
using Transferdata.API.Resources.PostDay;

namespace Transferdata.API.Services
{
    public class PostDayService : IPostDayService
    {
        private readonly IPostDayRepository _postdayRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PostDayService(
            IPostDayRepository postdayRepository,
            IUnitOfWork unitOfWork
            )
        {
            _postdayRepository = postdayRepository;
            _unitOfWork = unitOfWork;

        }

        public async Task<DepositResponse> GetDepositAmt(PostDayQueryResource query)
        {
            return await _postdayRepository.GetDepositAmt(query);
        }
    }
}
