using Report.API.Domain.Models.Requests;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Services
{
    public class PostDayService : IPostDayService
    {
        private readonly IPostDayRepository _repository;

        public PostDayService(IPostDayRepository repo)
        {
            _repository = repo;
        }

        public MemoryStream GetWorkDateExcel(PostDayRequest req)
        {
            return _repository.GetWorkDateExcel(req);
        }
    }
}
