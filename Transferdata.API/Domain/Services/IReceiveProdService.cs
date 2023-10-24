﻿using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;

namespace Transferdata.API.Domain.Services
{

    public interface IReceiveProdService
    {
        Task<List<InvReceiveProdHd>> ListReceiveProdAsync(ReceiveProdQuery query);
        Task<List<InvReceiveProdHd>> ListReceiveOilAsync(ReceiveProdQuery query);
        Task<List<InvReceiveProdHd>> ListReceiveGasAsync(ReceiveProdQuery query);
    }
}
