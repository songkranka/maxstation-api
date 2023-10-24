﻿using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;

namespace Transferdata.API.Domain.Services
{
    public interface IReceiveService
    {
        Task<List<FinReceiveHd>> ListReceiveAsync(ReceiveQuery query);
    }
}
