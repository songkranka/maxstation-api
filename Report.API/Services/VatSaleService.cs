using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using Report.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Services
{
    public class VatSaleService : IVatSaleService
    {
        private readonly IVatSaleRepository _vatsaleRepository;
        public VatSaleService(IVatSaleRepository vatsaleRepository)
        {
            _vatsaleRepository = vatsaleRepository;
        }


        public VatSaleResponse GetVatSalePDF(VatSaleRequest req)
        {
            return _vatsaleRepository.GetVatSalePDF(req);
        }

    }
}
