using Vatno.Worker.Context;
using Vatno.Worker.Domain.Models.Repositories;
using Vatno.Worker.Domain.Models.response;
using Vatno.Worker.Domain.Models.Services;
using Vatno.Worker.Models;

namespace Vatno.Worker.Services
{
    public class VatNoService : IVatNoService
    {
        private readonly IVatNoRepository _vatNoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPTMaxStationContext _context;


        public VatNoService(IVatNoRepository vatNoRepository, IPTMaxStationContext context, IUnitOfWork unitOfWork)
        {
            _vatNoRepository = vatNoRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<List<VatNoResponse>> VatExportLog()
            => await _vatNoRepository.VatNoAsync();

        public async Task<LogVatnoMaxme> SaveLogVatNoAsync(string fileName, string status, string? errorMsg)
        {
            var log = new LogVatnoMaxme
            {
                FileName = fileName,
                Status = status,
                ErrorMsg = errorMsg,
                CreatedDate = DateTime.UtcNow
            };

            _context.Set<LogVatnoMaxme>().Add(log);
            await _unitOfWork.CommitAsync();
            return log;
        }
    }
}