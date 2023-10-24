using DailyOperation.API.Domain.Models;
using DailyOperation.API.Domain.Services;
using DailyOperation.API.Helpers;
using DailyOperation.API.Repositories;
using DailyOperation.API.Services;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DailyOperation.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DemoController));
        private readonly IPosService _posService;

        public DemoController(PTMaxstationContext context, IPosService posService) : base(context)
        {
            _posService = posService;
        }


        public class Cash{
            public string JOURNAL_ID { get; set; }
            public DateTime? BUSINESS_DATE { get; set; }

        }





    }
}
