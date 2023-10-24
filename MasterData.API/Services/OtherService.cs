using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class OtherService : IOtherService
    {
        private readonly IOtherRepository _otherRepository;

        public OtherService(
            IOtherRepository otherRepository)
        {
            _otherRepository = otherRepository;
        }

        public List<MasMapping> GetMasMappingList(OtherRequest req)
        {
            return _otherRepository.GetMasMappingList(req);
        }

        public RespDocType GetPattern(OtherRequest req)
        {
            RespDocType resp = new RespDocType();
            resp = _otherRepository.GetPatternByDocType(req);
            if (resp.MasDocPattern != null)
            {
                string pattern = MappingPattern(req, resp.MasDocPattern.MasDocPatternDt);
                resp.Pattern = pattern;
            }
            return resp;
        }

        public String MappingPattern(OtherRequest req, List<MasDocPatternDt> masDtList)
        {
            if(req == null || masDtList == null || !masDtList.Any())
            {
                return string.Empty;
            }
            string result = string.Empty;
            Func<string, string> funcGetString = x => (x ?? string.Empty).Trim();
            foreach (MasDocPatternDt row in masDtList)
            {
                if(row == null)
                {
                    continue;
                }
                string strDocCode = funcGetString(row.DocCode);
                if (0.Equals(strDocCode.Length))
                {
                    continue;
                }
                switch (strDocCode)
                {
                    case "Brn":
                        result += funcGetString( req.BrnCode);
                        break;
                    case "Comp":
                        result += funcGetString( req.CompCode);
                        break;
                    case "dd":case "MM":case "yyyy":case "yy":                        
                        result += funcGetString(req.DocDate.ToString(strDocCode));
                        break;
                    case "[Pre]":
                        result += funcGetString(row.DocValue);
                        break;
                    case "[#]":
                        string strDocValue = funcGetString(row.DocValue);
                        int intCount = 0;
                        int.TryParse(strDocValue, out intCount);                        
                        if(intCount > 0)
                        {
                            result += new string('#', intCount);
                        }
                        else
                        {
                            result += "#";
                        }
                        break;
                    case "-":
                        result += strDocCode;
                        break;
                }
            }
            return result;
        }
        /*
        public String MappingPattern(OtherRequest req, List<MasDocPatternDt> masDtList)
        {
            string pattern = "";
            foreach (MasDocPatternDt row in masDtList)
            {
                DateTime date = DateTime.Now;
                switch (row.DocCode)
                {
                    // กรุณาเรียงตามตัวอักษร
                    case "Brn":
                        pattern += req.BrnCode;
                        break;
                    case "Comp":
                        pattern += req.CompCode;
                        break;
                    case "dd":
                        pattern += req.DocDate.ToString("dd");
                        break;
                    case "MM":
                        pattern += req.DocDate.ToString("MM");
                        break;
                    case "yyyy":
                        pattern += req.DocDate.ToString("yyyy");
                        break;
                    case "yy":
                        pattern += req.DocDate.ToString("yy");
                        break;
                    case "[Pre]":
                        pattern += row.DocValue;
                        break;
                    case "[#]":
                        int count = int.Parse(row.DocValue);
                        for (int i = 0; i < count; i++)
                        {
                            pattern += "#";
                        }
                        break;
                    case "-":
                        pattern += "-";
                        break;
                }
            }
            return pattern;
        }
        */
    }
}
