using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Response;
using Transferdata.API.Domain.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;
using Transferdata.API.Resources.Pos;

namespace Transferdata.API.Services
{
    public class PosService : IPosService
    {
        private readonly IPosRepository _posRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PosService(
            IPosRepository posRepository,
            IUnitOfWork unitOfWork
            )
        {
            _posRepository = posRepository;
            _unitOfWork = unitOfWork;

        }

        public async Task<TranferPosResponse> GetDepositAmt(TranferPosQueryResource query)
        {
            return await _posRepository.GetDepositAmt(query);
        }
    }
}
