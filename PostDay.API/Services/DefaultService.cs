using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PostDay.API.Services
{
    public class DefaultService
    {

        public class GetRunningInput<T> {
            public string CompCode { get; set; }
            public string BrnCode { get; set; }
            public MasDocPatternDt[] ArrayDocPattern { get; set; }   
            public DateTime? DocDate { get; set; }
            public IQueryable<T> IQueryable { get; set; }
            public Expression<Func<T,bool>> FuncFilterYear { get; set; }
            public Expression<Func<T, bool>> FuncFilterMonth { get; set; }
            public Expression<Func<T, bool>> FuncFilterDay { get; set; }
            public Expression<Func<T, int>> FuncFindMax { get; set; }
            public Func<string,Task<bool>> FuncWhileAnyAsync { get; set; }
        }
        public class GetRunningOutPut
        {
            public int RunningNumber { get; set; }
            public String DocNo { get; set; }

            public string DocPattern { get; set; }
        }
        private const string _strAppJson = "application/json";
        private static PTMaxstationContext _context = null;
       
        private static Dictionary<string, string> _dicMapStatus= new Dictionary<string, string>() { 
            {"แอคทีฟ","Active" },
            {"สร้าง","New" },
            {"รออนุมัติ","Wait" },
            {"พร้อมใช้","Ready" },
            {"เอกสารถูกอ้างอิง","Reference" },
            {"ยกเลิก","Cancel" },
        };

        public DefaultService(PTMaxstationContext pContext)
        {
            _context = pContext;
        }

        public static async Task<IActionResult> DoActionAsync<T>(string pStrFunctionName, Func<Task<T>> pFunc , ILog pLog)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                T result;
                result = await pFunc();
                sw.Stop();                
                //pLog?.Info(pStrFunctionName + " : " + sw.ElapsedMilliseconds.ToString());
                return jsonResult(result);
            }
            catch (Exception ex)
            {
                //pLog?.Error(pStrFunctionName, ex);
                return exeptionResult(ex);
            }
        }
        public static IActionResult DoAction<T>(string pStrFunctionName, Func<T> pFunc, ILog pLog)
        {
            try
            {
                T result;
                result = pFunc();
                //pLog?.Info(pStrFunctionName + " Complete");
                return jsonResult(result);
            }
            catch (Exception ex)
            {
                //pLog?.Error(pStrFunctionName, ex);
                return exeptionResult(ex);
            }
        }
        public static async Task<GetRunningOutPut> GetRunning<T>(GetRunningInput<T> pInput)
        {
            string strRunningDocNo = string.Empty;
            string strRunningPattern = string.Empty;
            MasDocPatternDt[] arrDocPatternDetail = null;
            arrDocPatternDetail = pInput.ArrayDocPattern;
            bool isUseDefaultPattern = true;
            isUseDefaultPattern = arrDocPatternDetail == null || !arrDocPatternDetail.Any();
            int intLastRunning = 0;
            int intDay = 0;
            int intMonth = 0;
            int intYear = 0;
            if (pInput.DocDate.HasValue)
            {
                intDay = pInput.DocDate.Value.Day;
                intMonth = pInput.DocDate.Value.Month;
                intYear = pInput.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
            IQueryable<T> qry = pInput.IQueryable;
            if (isUseDefaultPattern)
            {
                qry = qry.Where(pInput.FuncFilterYear).Where(pInput.FuncFilterMonth);
            }
            else
            {
                arrDocPatternDetail = arrDocPatternDetail.OrderBy(x => x.SeqNo).ToArray();
                if (arrDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    qry = qry.Where(pInput.FuncFilterYear);
                    if (arrDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        qry = qry.Where(pInput.FuncFilterMonth);                        
                        if (arrDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            qry = qry.Where(pInput.FuncFilterDay);
                        }
                    }
                }
            }

            if (await qry.AnyAsync())
            {
                int intMaxRunning = await qry.MaxAsync(pInput.FuncFindMax);
                int intRowCount = await qry.CountAsync();
                intLastRunning = Math.Max(intMaxRunning, intRowCount);
            }
            do
            {
                strRunningDocNo = string.Empty;
                intLastRunning++;
                if (isUseDefaultPattern)
                {
                    strRunningDocNo = string.Format("{0}{1}{2:D5}", intYear, intMonth, intLastRunning);
                }
                else
                {
                    foreach (var item in arrDocPatternDetail)
                    {
                        if (item == null) continue;
                        switch (item.DocCode)
                        {
                            case "-": strRunningDocNo += "-"; break;
                            case "MM": strRunningDocNo += intMonth.ToString("00"); break;
                            case "Comp": strRunningDocNo += pInput.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pInput.BrnCode; break;
                            case "yyyy": strRunningDocNo += intYear.ToString("0000"); break;
                            case "yy": strRunningDocNo += intYear.ToString().Substring(2, 2); break;
                            case "[#]":
                                int intDocValue = 0;
                                int.TryParse(item.DocValue, out intDocValue);
                                strRunningDocNo += intLastRunning.ToString().PadLeft(intDocValue, '0');
                                strRunningPattern += new string('#', intDocValue);
                                break;
                            default:
                                break;
                        }
                        if (!"[#]".Equals(item.DocCode))
                        {
                            strRunningPattern = strRunningDocNo;
                        }
                    }
                }
            } while (await pInput.FuncWhileAnyAsync(strRunningDocNo));
            GetRunningOutPut result = null;
            result = new GetRunningOutPut()
            {
                DocNo = strRunningDocNo,
                RunningNumber = intLastRunning,
                DocPattern = strRunningPattern,
            };
            return result;
        }
        public static async Task<MasDocPatternDt[]> GetArrayDocPattern(string pStrDocType , PTMaxstationContext pContex)
        {
            pStrDocType = (pStrDocType ?? string.Empty).Trim();
            if (0.Equals(pStrDocType.Length))
            {
                return null;
            }
            IQueryable<MasDocPatternDt> qryDocPattern = null;
            qryDocPattern = from dp in pContex.MasDocPatterns.AsNoTracking()
                            join dt in pContex.MasDocPatternDts.AsNoTracking()
                            on dp.DocId equals dt.DocId
                            where pStrDocType.Equals(dp.DocType)
                            select new MasDocPatternDt()
                            {
                                DocValue = dt.DocValue,
                                DocCode = dt.DocCode,
                                SeqNo = dt.SeqNo
                            };
            MasDocPatternDt[] result = null;
            result = await qryDocPattern.ToArrayAsync();
            return result;
        }
        public static string EncodeSqlString(object pInput, string pStrDefault = "")
        {
            string result = string.Empty;
            result = GetString(pInput, pStrDefault).Replace("'", "''");
            return result;
        }
        public static object GetDefaultDbNull(object pInput)
        {
            if(pInput == null)
            {
                return DBNull.Value;
            }
            else
            {
                return pInput;
            }
        }
        public static string GetString(object pInput , string pStrDefault = "")
        {
            if(pInput == null || Convert.IsDBNull(pInput))
            {
                return pStrDefault;
            }
            string result = (pInput?.ToString() ?? string.Empty).Trim();
            if (0.Equals(result.Length))
            {
                return pStrDefault;
            }
            return result;
        }        
        public static decimal? GetDecimal(object pInput , decimal? pDecDefault = decimal.Zero)
        {
            string strInput = GetString(pInput);
            decimal result = decimal.Zero;
            if(decimal.TryParse(strInput , out result))
            {
                return result;
            }
            else
            {
                return pDecDefault;
            }
        }
        private static ContentResult jsonResult(object pInput)
        {
            string strJson = string.Empty;

            strJson = JsonConvert.SerializeObject(pInput);
            ContentResult result = null;
            result = new ContentResult() { 
                Content = strJson ,
                ContentType = _strAppJson
            };
            
            return result;
        }
        private static string getErrorMessage(Exception pException)
        {
            if (pException == null)
            {
                return string.Empty;
            }
            string result = string.Empty;
            result = pException.StackTrace;
            while (pException.InnerException != null)
            {
                pException = pException.InnerException;
            }
            result = pException.Message + Environment.NewLine + result;
            return result;
        }
        private static BadRequestObjectResult exeptionResult(Exception pException)
        {
            string strErrorMessage = string.Empty;
            strErrorMessage = getErrorMessage(pException);
            BadRequestObjectResult result = new BadRequestObjectResult(strErrorMessage);
            return result;
        }
        public static T ConvertObject<T>(object pObjInput)
        {
            if (pObjInput == null)
            {
                return default(T);
            }
            string strSerialized = JsonConvert.SerializeObject(pObjInput);
            T result = JsonConvert.DeserializeObject<T>(strSerialized);
            return result;
        }
        public static T CloneObject<T>(T pInput)
        {
            T result = ConvertObject<T>(pInput);
            return result;
        }
        public static List<string> GetListDocStatusFromKeyWord(string pStrKeyWord)
        {
            pStrKeyWord = (pStrKeyWord ?? string.Empty).Trim();
            if (0.Equals(pStrKeyWord))
            {
                return null;
            }
            List<string> result = new List<string>();
            foreach (var item in _dicMapStatus.Keys)
            {
                if (pStrKeyWord.Contains(item))
                {
                    result.Add(_dicMapStatus[item]);
                }
            }
            return result;
        }
        public static async Task< DataTable> GetDataTable(string pStrConnection , string pStrSql)
        {
            pStrConnection = GetString(pStrConnection);
            pStrSql = GetString(pStrSql);
            if(0.Equals( pStrConnection.Length) || 0.Equals(pStrSql.Length))
            {
                return null;
            }
            DataTable result = new DataTable();
            using(var da = new SqlDataAdapter(pStrSql , pStrConnection))
            {
                await Task.Run(() => da.Fill(result));
            }
            return result;
        }
        public static T GetEntityFromDataTable<T>(DataTable pDataTable)
        {
            if(pDataTable == null || 0.Equals(pDataTable.Rows.Count))
            {
                return default(T);
            }
            Func<string, string> funcMapColName = null;
            funcMapColName = x =>
            {
                string[] arrSplit = x.Split("_");
                for (int i = 0; i < arrSplit.Length; i++)
                {
                    arrSplit[i] = arrSplit[i].ToLower();
                    char[] arrChar = arrSplit[i].ToCharArray();
                    arrChar[0] = arrChar[0].ToString().ToUpper()[0];
                    arrSplit[i] = new string(arrChar);
                }
                return string.Join(string.Empty,arrSplit);
            };
            List<DataColumn> arrOriginalCol = pDataTable.Columns.OfType<DataColumn>().ToList();
            foreach (DataColumn item in pDataTable.Columns)
            {
                item.ColumnName = funcMapColName(item.ColumnName);
            }
            T result = ConvertObject<T>(pDataTable);
            return result;
        }
        public static async Task< T> GetEntityFromSql<T>(string pStrConnection ,string pStrSql)
        {
            T result = default(T);
            using (DataTable dt = await GetDataTable(pStrConnection, pStrSql))
            {
                result = GetEntityFromDataTable<T>(dt);
            }
            return result;
        }
        public static DataTable GetDataTableFromIenum<T>(IEnumerable<T> pInput)
        {
            DataTable result = ConvertObject<DataTable>(pInput);
            Regex r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
            foreach (DataColumn col in result.Columns)
            {
                col.ColumnName = r.Replace(col.ColumnName, "_").ToUpper();
            }
            return result;
        }
        public static async Task<T> ExecuteScalar<T>(string pStrConnection, string pStrSql)
        {
            pStrConnection = GetString(pStrConnection);
            pStrSql = GetString(pStrSql);
            if (0.Equals(pStrConnection.Length) || 0.Equals(pStrSql.Length))
            {
                return default(T);
            }
            T result = default(T);
            using(var con = new SqlConnection(pStrConnection))
            using (var com = new SqlCommand(pStrSql , con ))
            {
                if(con.State != ConnectionState.Open)
                {
                    await con.OpenAsync();
                }
                result = (T)await com.ExecuteScalarAsync();
            }
            return result;
        }
        public static async Task<int> ExecuteNonQuery(string pStrConnection, string pStrSql)
        {
            pStrConnection = GetString(pStrConnection);
            pStrSql = GetString(pStrSql);
            if (0.Equals(pStrConnection.Length) || 0.Equals(pStrSql.Length))
            {
                return 0;
            }
            int result = 0;
            using (var con = new SqlConnection(pStrConnection))
            using (var com = new SqlCommand(pStrSql, con))
            {
                if (con.State != ConnectionState.Open)
                {
                    await con.OpenAsync();
                }
                result = await com.ExecuteNonQueryAsync();
            }
            return result;
        }
        public static async Task<V> CallPostApi<T, V>(string pStrUrl, T pInput)
        {
            string strJson = JsonConvert.SerializeObject(pInput);
            string strJsonResult = string.Empty;
            using (HttpClient client = new HttpClient())
            using (StringContent sc = new StringContent(
                content: strJson,
                encoding: Encoding.UTF8,
                mediaType: "application/json"
            ))
            using (HttpResponseMessage postResult = await client.PostAsync(
                requestUri: pStrUrl,
                content: sc
            ))
            {
                if (postResult != null && postResult.IsSuccessStatusCode)
                {
                    strJsonResult = await postResult.Content.ReadAsStringAsync();
                }
            }
            V result = default(V);
            if (!string.IsNullOrWhiteSpace(strJsonResult))
            {
                result = JsonConvert.DeserializeObject<V>(strJsonResult);
            }
            return result;
        }
        #region - GetApproverStep -
        public static async Task<GetApproverStepResult> GetApproverStep(GetApproverStepParam param)
        {
            string strUrl = "http://172.16.112.205:8080/HrisService/workflow-getapproverstep-dynamic";
            GetApproverStepResult result = null;
            result = await CallPostApi<GetApproverStepParam, GetApproverStepResult>(strUrl, param);
            return result;
        }
        public static async Task<GetApproverStepResult> GetApproverStep(int pIntStepQty, string pStrEmpCode, PTMaxstationContext pContext = null)
        {
            GetApproverStepParam param = new GetApproverStepParam()
            {
                empidRequest = pStrEmpCode,
                stepQty = pIntStepQty
            };
            //string strUrl = "http://172.16.112.205:8080/HrisService/workflow-getapproverstep-dynamic";
            var strUrl = "";
            GetApproverStepResult result = null;
            if (pContext != null)
            {
                var qryConfig = pContext.SysConfigApis.Where(x => x.ApiId == "M001" && x.SystemId == "HRIS");
                var configApi = await qryConfig.FirstOrDefaultAsync();
                strUrl = GetString(configApi?.ApiUrl);
                if (strUrl == "")
                {
                    result = await getApproverStep3(pStrEmpCode, pIntStepQty, pContext);
                }
                else
                {
                    result = await CallPostApi<GetApproverStepParam, GetApproverStepResult>(strUrl, param);
                }
            }
            return result;
        }
        private static async Task<GetApproverStepResult> getApproverStep3(string pStrEmpCode, int pIntStepQty, PTMaxstationContext pContext = null)
        {
            if (string.IsNullOrWhiteSpace(pStrEmpCode) || pContext == null)
            {
                return null;
            }
            var strEmpCode = EncodeSqlString(pStrEmpCode);
            string strSql = @$";with cte as (
	select HEAD1_ID empId , 1 stepSeq  from mas_employee_level(nolock) where emp_id = '{strEmpCode}'
	union all
	select HEAD1_ID empId , stepSeq +1 stepSeq from mas_employee_level(nolock) join cte on EMP_ID = cte.empId
	where HEAD1_ID is not null and stepSeq < {pIntStepQty}
)select * from cte;";
            string strCon = pContext.Database.GetConnectionString();
            var arrWorkFlow = await GetEntityFromSql<WorkflowApprover[]>(strCon, strSql);
            var result = new GetApproverStepResult()
            {
                workflowApprover = arrWorkFlow,
                empidRequest = pStrEmpCode
            };
            return result;
        }

        public class GetApproverStepParam
        {
            public int stepQty { get; set; }
            public string empidRequest { get; set; }
        }

        public class WorkflowApprover
        {
            public int stepSeq { get; set; }
            public string empId { get; set; }
            public string numLvl { get; set; }
            public string codcomp { get; set; }
        }
        public class GetApproverStepResult
        {
            public string transactionId { get; set; }
            public string serviceId { get; set; }
            public string workflowId { get; set; }
            public string empidRequest { get; set; }
            public WorkflowApprover[] workflowApprover { get; set; }
        }


        #endregion

    }


}
