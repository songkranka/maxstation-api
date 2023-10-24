using MaxStation.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;

namespace Transferdata.API.Services
{
    public class TaxInvoiceService : ITaxInvoiceService
    {
        private readonly ITaxInvoiceRepository _taxinvoiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private PTMaxstationContext Context;

        public TaxInvoiceService(
            ITaxInvoiceRepository taxinvoiceRepository,
            IUnitOfWork unitOfWork,
            PTMaxstationContext context,
            IServiceScopeFactory serviceScopeFactory)
        {
            _taxinvoiceRepository = taxinvoiceRepository;
            _unitOfWork = unitOfWork;
            Context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<List<SalTaxinvoiceHd>> ListTaxInvoiceAsync(TaxInvoiceQuery query)
        {
            return await this._taxinvoiceRepository.ListTaxInvoiceAsync(query);
        }
    }
}
