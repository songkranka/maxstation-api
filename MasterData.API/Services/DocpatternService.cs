using MasterData.API.Domain.Models.DocPattern;
using MasterData.API.Domain.Models.DocPattern.Request;
using MasterData.API.Domain.Models.DocPattern.Response;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using System;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class DocpatternService : IDocpatternService
    {
        private readonly IDocpatternRepository _docPatternRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DocpatternService(
            IDocpatternRepository docPatternRepository,
            IUnitOfWork unitOfWork)
        {
            _docPatternRepository = docPatternRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DocPatternResponse> GetDocPatternAsync(DocPatternRequest request)
        {
            var response = new DocPattern();

            try
            {
                var listDocPatternDetail =  await _docPatternRepository.FindDocPatternDtByDocTypeAsync(request.DocType);
                int intDay = 0;
                int intMonth = 0;
                int intYear = 0;

                if (request.DocDate != null && request.DocDate.HasValue)
                {
                    intDay = request.DocDate.Value.Day;
                    intMonth = request.DocDate.Value.Month;
                    intYear = request.DocDate.Value.Year;
                }
                else
                {
                    intDay = DateTime.Now.Day;
                    intMonth = DateTime.Now.Month;
                    intYear = DateTime.Now.Year;
                }

                var strRunningDocNo = string.Empty;
                foreach (var item in listDocPatternDetail)
                {
                    if (item == null) continue;
                    switch (item.DocCode)
                    {
                        case "-": strRunningDocNo += "-"; break;
                        case "MM": strRunningDocNo += intMonth.ToString("00"); break;
                        case "Comp": strRunningDocNo += request.CompCode; break;
                        case "[Pre]": strRunningDocNo += item.DocValue; break;
                        case "dd": strRunningDocNo += intDay.ToString("00"); break;
                        case "Brn": strRunningDocNo += request.BrnCode; break;
                        case "yyyy": strRunningDocNo += intYear.ToString("0000"); break;
                        case "yy": strRunningDocNo += intYear.ToString().Substring(2, 2); break;
                        case "[#]":
                            int intDocValue = 0;
                            int.TryParse(item.DocValue, out intDocValue);
                            strRunningDocNo = strRunningDocNo.PadRight(strRunningDocNo.Length + intDocValue, '#');
                            break;
                        default:
                            break;
                    }
                }


                response.Pattern = strRunningDocNo;
                return new DocPatternResponse(response);
            }
            catch (Exception ex)
            {
                return new DocPatternResponse($"An error occurred when get the docPattern: {ex.Message}");
            }
        }
    }
}
