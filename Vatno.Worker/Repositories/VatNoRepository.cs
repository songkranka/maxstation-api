using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Vatno.Worker.Context;
using Vatno.Worker.Domain.Models.Repositories;
using Vatno.Worker.Domain.Models.response;
using Vatno.Worker.Dto;
using Vatno.Worker.Models;

namespace Vatno.Worker.Repositories
{
    public class VatNoRepository : IVatNoRepository
    {
        private readonly IPTMaxStationContext _context;

        public VatNoRepository(IPTMaxStationContext context)
        {
            _context = context;
        }

        private readonly MapperConfiguration _getBranchConfiguration = new(cfg => cfg.CreateProjection<MasBranch, BranchDto>()
            .ForMember(dto => dto.BrnCode, cfg => cfg.MapFrom(source => source.BrnCode))
            .ForMember(dto => dto.BrnName, cfg => cfg.MapFrom(source => source.BrnName))
            .ForMember(dto => dto.Address, cfg => cfg.MapFrom(source => source.Address))
            .ForMember(dto => dto.PostCode, cfg => cfg.MapFrom(source => source.Postcode))
            .ForMember(dto => dto.Phone, cfg => cfg.MapFrom(source => source.Phone))
            .ForMember(dto => dto.BranchNo, cfg => cfg.MapFrom(source => source.BranchNo))
            .ForMember(dto => dto.CompCode, cfg => cfg.MapFrom(source => source.CompCode))
        );

        private readonly MapperConfiguration _getCompanyConfiguration = new(cfg => cfg.CreateProjection<MasCompany, CompanyDto>()
            .ForMember(dto => dto.CompCode, cfg => cfg.MapFrom(source => source.CompCode))
            .ForMember(dto => dto.CompSname, cfg => cfg.MapFrom(source => source.CompSname))
            .ForMember(dto => dto.CompName, cfg => cfg.MapFrom(source => source.CompName))
            .ForMember(dto => dto.CompNameEn, cfg => cfg.MapFrom(source => source.CompName))
            .ForMember(dto => dto.Address, cfg => cfg.MapFrom(source => source.Address))
            .ForMember(dto => dto.AddressEn, cfg => cfg.MapFrom(source => source.AddressEn))
            .ForMember(dto => dto.Phone, cfg => cfg.MapFrom(source => source.Phone))
            .ForMember(dto => dto.Fax, cfg => cfg.MapFrom(source => source.Fax))
            .ForMember(dto => dto.RegisterId, cfg => cfg.MapFrom(source => source.RegisterId))
        );


        public async Task<List<VatNoResponse>> VatNoAsync()
        {
            var masterBranches = _context.Set<MasBranch>().ProjectTo<BranchDto>(_getBranchConfiguration);

            var masterCompanies = _context.Set<MasCompany>().ProjectTo<CompanyDto>(_getCompanyConfiguration);

            var result = await masterBranches.SelectMany(br => masterCompanies.Where(b => b.CompCode == br.CompCode),
                (br, cr) => new VatNoResponse
                {
                    brnCode = br.BrnCode,
                    brnName = br.BrnName,
                    brnAddress = br.Address,
                    brnPostcode = br.PostCode,
                    brnPhone = br.Phone.Trim(),
                    brnCompSname = cr.CompSname ?? string.Empty,
                    brnCodeNew = br.BrnCode.Length > 0 && br.BrnCode.Length == 3 ? "52" + br.BrnCode.Substring(0, 3) : br.BrnCode,
                    brnBranchNo = br.BranchNo,
                    compName = cr.CompName ?? string.Empty,
                    compAddress = cr.Address ?? string.Empty,
                    compPhoneFax = !string.IsNullOrWhiteSpace(cr.Phone) && !string.IsNullOrWhiteSpace(cr.Fax) ? $"โทรศัพท์ {cr.Phone.Trim()} โทรสาร {cr.Fax.Trim()}" : !string.IsNullOrWhiteSpace(cr.Phone) && string.IsNullOrWhiteSpace(cr.Fax) ? $"โทรศัพท์ {cr.Phone.Trim()}" : !string.IsNullOrWhiteSpace(cr.Fax) && string.IsNullOrWhiteSpace(cr.Phone) ? $"โทรสาร {cr.Fax.Trim()}" : string.Empty,
                    compRegisterId = cr.RegisterId ?? string.Empty,
                    compNameEn = cr.CompNameEn ?? string.Empty,
                    compAddressEn = cr.AddressEn ?? string.Empty,
                    compPhoneFaxEn = !string.IsNullOrWhiteSpace(cr.Phone) && !string.IsNullOrWhiteSpace(cr.Fax) ? $"TEL {cr.Phone.Trim()} FAX {cr.Fax.Trim()}" : !string.IsNullOrWhiteSpace(cr.Phone) && string.IsNullOrWhiteSpace(cr.Fax) ? $"TEL {cr.Phone.Trim()}" : !string.IsNullOrWhiteSpace(cr.Fax) && string.IsNullOrWhiteSpace(cr.Phone) ? $"FAX {cr.Fax.Trim()}" : string.Empty
                }).ToListAsync();
            return result.AsEnumerable().OrderBy(o => o.brnCode).ToList();
        }
    }
}