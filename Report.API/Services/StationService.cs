using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Report.API.Domain.Models.Response.StationResponse;

namespace Report.API.Services
{
    public class StationService : IStationService
    {
        private readonly IStationRepository _stationRepository;
        public StationService(IStationRepository stationRepository)
        {
            _stationRepository = stationRepository;
        }

        public MemoryStream GetST310Excel(StationRequest req)
        {
            return _stationRepository.GetST310Excel(req);
        }

        public List<ST312Response> GetST312PDF(StationRequest req)
        {
            return _stationRepository.GetST312PDF(req);
        }

        public MemoryStream GetST313Excel(StationRequest req)
        {
            return _stationRepository.GetST313Excel(req);
        }

        public List<ST313Response> GetST313PDF(StationRequest req)
        {
            return _stationRepository.GetST313PDF(req);
        }

        public List<ST315Response> GetST315PDF(StationRequest req)
        {
            return _stationRepository.GetST315PDF(req);
        }

        public MemoryStream GetST315Excel(StationRequest req)
        {
            return _stationRepository.GetST315Excel(req);
        }

        public MemoryStream GetST316Excel(StationRequest req)
        {
            return _stationRepository.GetST316Excel(req);
        }

        public List<ST316Response> GetST316PDF(StationRequest req)
        {
            return _stationRepository.GetST316PDF(req);
        }

        public MemoryStream GetST317Excel(StationRequest req)
        {
            return _stationRepository.GetST317Excel(req);
        }

        public List<ST317Response> GetST317PDF(StationRequest req)
        {
            return _stationRepository.GetST317PDF(req);
        }
    }
}
