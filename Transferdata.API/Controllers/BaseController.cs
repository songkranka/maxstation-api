﻿using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transferdata.API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected PTMaxstationContext context;

        public BaseController(PTMaxstationContext context)
        {
            this.context = context;
        }

    }
}
