using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyOperation.API.Services
{
    public class LogErrorService
    {

        private static string _strBody = string.Empty;
        private static string _strUrlPath = string.Empty;
        private static string _strHost = string.Empty;
        private static string _strMethod = string.Empty;
        private static HttpContext _httpContext = null;
        private static string _strConnectionString = string.Empty;
        private static SqlConnection _sqlCon = null;

        private static IHttpContextAccessor _httpContextAccessor = null;

        private const string _strKeyJsonBody = "LogErrorServiceJsonBody";
        private const string _strKeyUrlPath = "LogErrorServiceUrlPath";
        private const string _strKeyHost = "LogErrorServiceHost";
        private const string _strKeyMethod = "LogErrorServiceMethod";

        public LogErrorService()
        {

        }

        public static void SetHttpContextAccessor(IHttpContextAccessor accessor)
        {
            _httpContextAccessor = accessor;
        }

        public static void SetConnectionString(string pStrConnection)
        {
            pStrConnection = (pStrConnection ?? string.Empty).Trim();
            _strConnectionString = pStrConnection;
        }

        public static async Task SetHttpContext(HttpContext pContext)
        {

            if (pContext == null)
            {
                return;
            }
            pContext.Request.EnableBuffering();
            pContext.Request.Body.Position = 0;
            var reader = new StreamReader(pContext.Request.Body, Encoding.UTF8);
            string strJsonBody = await reader.ReadToEndAsync();
            pContext.Request.Body.Position = 0;
            pContext.Items[_strKeyJsonBody] = strJsonBody;
            pContext.Items[_strKeyHost] = pContext.Request.Host.Value;
            pContext.Items[_strKeyMethod] = pContext.Request.Method;
            pContext.Items[_strKeyUrlPath] = pContext.Request.Path;

        }        
        public static async Task<bool> WriteErrorLog(Exception pException)
        {
            if (pException == null)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(_strConnectionString))
            {
                return false;
            }
            string strCommand = getSqlCommand(pException);

            using (var com = _sqlCon.CreateCommand())
            {
                com.CommandText = strCommand;
                try
                {
                    var intResult = await com.ExecuteNonQueryAsync();
                    return intResult > 0;
                }
                catch
                {
                    return false;
                }
            }
        }
        private static string getSqlCommand(Exception pException)
        {
            if (pException == null
            || _httpContextAccessor == null
            || _httpContextAccessor.HttpContext == null)
            {
                return string.Empty;
            }
            string strMessage = string.Empty;
            string strStacktrace = string.Empty;
            while (pException.InnerException != null)
            {
                pException = pException.InnerException;
            }


            var context = _httpContextAccessor.HttpContext;
            string strJsonBody = context.Items[_strKeyJsonBody]?.ToString() ?? string.Empty;
            string strUrlPath = context.Items[_strKeyUrlPath]?.ToString() ?? string.Empty;
            string strMethod = context.Items[_strKeyMethod]?.ToString() ?? string.Empty;
            string strHost = context.Items[_strKeyHost]?.ToString() ?? string.Empty;
            strMessage = pException.Message;
            strStacktrace = pException.StackTrace;

            string strCompCode = string.Empty;
            string strLocCode = string.Empty;
            string strBrnCode = string.Empty;
            string strCreateBy = string.Empty;

            readJson(
                pStrJson: strJsonBody,
                pAcGetBrnCode: x => strBrnCode = x,
                pAcGetCompCode: x => strCompCode = x,
                pAcGetCreateBy: x => strCreateBy = x,
                pAcGetLocCode: x => strLocCode = x
            );


            string result = $@"INSERT INTO [dbo].[LOG_ERROR]
           ([LOG_STATUS]
           ,[COMP_CODE]
           ,[BRN_CODE]
           ,[LOC_CODE]
           ,[JSON_DATA]
           ,[MESSAGE]
           ,[CREATED_DATE]
           ,[CREATED_BY]
           ,[PATH]
           ,[METHOD]
           ,[HOST]
           ,[STACK_TRACE])
     VALUES
           (
		    'Error'--<LOG_STATUS, varchar(50),>
           ,'{DefaultService.EncodeSqlString(strCompCode)}'--<COMP_CODE, varchar(20),>
           ,'{DefaultService.EncodeSqlString(strBrnCode)}'--<BRN_CODE, varchar(20),>
           ,'{DefaultService.EncodeSqlString(strLocCode)}'--<LOC_CODE, varchar(20),>
           ,'{DefaultService.EncodeSqlString(strJsonBody)}'--<JSON_DATA, varchar(max),>
           ,'{DefaultService.EncodeSqlString(strMessage)}'--<MESSAGE, varchar(max),>
           ,'{DefaultService.EncodeSqlDateTime(DateTime.Now)}'--<CREATED_DATE, datetime,>
           ,'{DefaultService.EncodeSqlString(strCreateBy)}'--<CREATED_BY, varchar(50),>
           ,'{DefaultService.EncodeSqlString(strUrlPath)}'--<PATH, varchar(100),>
           ,'{DefaultService.EncodeSqlString(strMethod)}'--<METHOD, varchar(10),>
           ,'{DefaultService.EncodeSqlString(strHost)}'--<HOST, varchar(20),>
           ,'{DefaultService.EncodeSqlString(strStacktrace)}')--<STACK_TRACE, varchar(max),>)";
            return result;
        }
        private static string getSqlCommandOld(Exception pException)
        {
            string strMessage = string.Empty;
            string strStacktrace = string.Empty;
            while (pException.InnerException != null)
            {
                pException = pException.InnerException;
            }

            if (_httpContextAccessor == null || _httpContextAccessor.HttpContext == null)
            {
                return string.Empty;
            }
            var context = _httpContextAccessor.HttpContext;
            string strXX = context.Items["json"].ToString();

            strMessage = pException.Message;
            strStacktrace = pException.StackTrace;
            string result = $@"INSERT INTO [dbo].[LOG_ERROR]
           ([LOG_STATUS]
           ,[COMP_CODE]
           ,[BRN_CODE]
           ,[LOC_CODE]
           ,[JSON_DATA]
           ,[MESSAGE]
           ,[CREATED_DATE]
           ,[CREATED_BY]
           ,[PATH]
           ,[METHOD]
           ,[HOST]
           ,[STACK_TRACE])
     VALUES
           (
		    'Error'--<LOG_STATUS, varchar(50),>
           ,''--<COMP_CODE, varchar(20),>
           ,''--<BRN_CODE, varchar(20),>
           ,''--<LOC_CODE, varchar(20),>
           ,'{DefaultService.EncodeSqlString(strXX)}'--<JSON_DATA, varchar(max),>
           ,'{DefaultService.EncodeSqlString(strMessage)}'--<MESSAGE, varchar(max),>
           ,'{DefaultService.EncodeSqlDateTime(DateTime.Now)}'--<CREATED_DATE, datetime,>
           ,''--<CREATED_BY, varchar(50),>
           ,'{DefaultService.EncodeSqlString(_strUrlPath)}'--<PATH, varchar(100),>
           ,'{DefaultService.EncodeSqlString(_strMethod)}'--<METHOD, varchar(10),>
           ,'{DefaultService.EncodeSqlString(_strHost)}'--<HOST, varchar(20),>
           ,'{DefaultService.EncodeSqlString(strStacktrace)}')--<STACK_TRACE, varchar(max),>)";
            return result;
        }
        public static async Task<bool> ConnectServer()
        {
            if (string.IsNullOrWhiteSpace(_strConnectionString))
            {
                return false;
            }
            if (_sqlCon == null)
            {
                _sqlCon = new SqlConnection(_strConnectionString);
            }
            try
            {
                if (_sqlCon.State != System.Data.ConnectionState.Open)
                {
                    await _sqlCon.OpenAsync();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public static async Task DisConnectServer()
        {
            if (_sqlCon == null)
            {
                return;
            }
            await _sqlCon.CloseAsync();
        }

        private static void readJson(
            string pStrJson,
            Action<string> pAcGetCompCode,
            Action<string> pAcGetLocCode,
            Action<string> pAcGetBrnCode,
            Action<string> pAcGetCreateBy
        )
        {
            pStrJson = (pStrJson ?? string.Empty).Trim();
            if (pStrJson.Length == 0)
            {
                return;
            }
            string strCompCode = string.Empty;
            string strLocCode = string.Empty;
            string strBrnCode = string.Empty;
            string strCreateBy = string.Empty;
            var jObj = JObject.Parse(pStrJson);
            var listJtk = allTokens(jObj).ToList();
            Func<bool> funcIsContinue = ()
                => string.IsNullOrEmpty(strCompCode)
                || string.IsNullOrEmpty(strBrnCode)
                || string.IsNullOrEmpty(strLocCode)
                || string.IsNullOrEmpty(strCreateBy);

            foreach (var item in listJtk)
            {
                if (!funcIsContinue())
                {
                    break;
                }
                string strPropName = (item as JProperty)?.Name ?? string.Empty;

                switch (strPropName.ToLower())
                {
                    case "compcode":

                        strCompCode = item.ToArray()[0].ToString();
                        break;
                    case "brncode":
                        strBrnCode = item.ToArray()[0].ToString();
                        break;
                    case "loccode":
                        strLocCode = item.ToArray()[0].ToString();
                        break;
                    case "createdby":
                        strCreateBy = item.ToArray()[0].ToString();
                        break;

                    default:


                        break;
                }
            }
            if (pAcGetBrnCode != null)
            {
                pAcGetBrnCode(strBrnCode);
            }
            if (pAcGetCompCode != null)
            {
                pAcGetCompCode(strCompCode);
            }
            if (pAcGetLocCode != null)
            {
                pAcGetLocCode(strLocCode);
            }
            if (pAcGetCreateBy != null)
            {
                pAcGetCreateBy(strCreateBy);
            }
        }

        private static void readJsonOld(
            string pStrJson,
            Action<string> pAcGetCompCode,
            Action<string> pAcGetLocCode,
            Action<string> pAcGetBrnCode,
            Action<string> pAcGetCreateBy
        )
        {
            pStrJson = (pStrJson ?? string.Empty).Trim();
            if (pStrJson.Length == 0)
            {
                return;
            }
            string strCompCode = string.Empty;
            string strLocCode = string.Empty;
            string strBrnCode = string.Empty;
            string strCreateBy = string.Empty;

            Func<bool> funcIsContinue = ()
                => string.IsNullOrEmpty(strCompCode)
                || string.IsNullOrEmpty(strBrnCode)
                || string.IsNullOrEmpty(strLocCode)
                || string.IsNullOrEmpty(strCreateBy)
                ;
            var jObj = JObject.Parse(pStrJson);
            var jToken = JToken.Parse(pStrJson);
            Action<JObject> acLoopJson = null;
            acLoopJson = j => {
                foreach (var item in j)
                {
                    if (!funcIsContinue())
                    {
                        break;
                    }
                    if (item.Value.Type == JTokenType.Object)
                    {
                        acLoopJson(item.Value.ToObject<JObject>());
                        break;
                    }
                    if (item.Value.Type == JTokenType.Array)
                    {
                        foreach (var item2 in item.Value.ToObject<JArray>())
                        {

                        }
                    }
                    switch (item.Key.ToLower())
                    {
                        case "compcode":
                            strCompCode = item.Value.ToString();
                            break;
                        case "brncode":
                            strBrnCode = item.Value.ToString();
                            break;
                        case "loccode":
                            strLocCode = item.Value.ToString();
                            break;
                        case "createdby":
                            strCreateBy = item.Value.ToString();
                            break;

                        default:


                            break;
                    }
                }
            };
            acLoopJson(jObj);
            if (pAcGetBrnCode != null)
            {
                pAcGetBrnCode(strBrnCode);
            }
            if (pAcGetCompCode != null)
            {
                pAcGetCompCode(strBrnCode);
            }
            if (pAcGetLocCode != null)
            {
                pAcGetLocCode(strBrnCode);
            }
            if (pAcGetCreateBy != null)
            {
                pAcGetCreateBy(strBrnCode);
            }
        }

        private static IEnumerable<JToken> allTokens(JObject obj)
        {
            var toSearch = new Stack<JToken>(obj.Children());
            while (toSearch.Count > 0)
            {
                var inspected = toSearch.Pop();
                yield return inspected;
                foreach (var child in inspected)
                {
                    toSearch.Push(child);
                }
            }
        }
    }
}
