﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Request
{
    public class AuthBranchRequest
    {
        public string CompCode { get; set; }
        public string Username { get; set; }
    }
}
