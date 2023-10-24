using DailyOperation.API.Domain.Models;
using DailyOperation.API.Domain.Models.Queries;
using DailyOperation.API.Domain.Models.Request;
using DailyOperation.API.Domain.Models.Response;
using DailyOperation.API.Domain.Repositories;
using DailyOperation.API.Helpers;
using DailyOperation.API.Resources.POS;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;
using MaxStation.Utility.Extensions;
using System.Reflection.Emit;
using Org.BouncyCastle.Ocsp;

namespace DailyOperation.API.Repositories
{
    public class PosRepository : SqlDataAccessHelper, IPosRepository
    {
        private readonly string _connectionString;
        private readonly HttpClient _client;
        private const string schema = "raptorpos.";//raptorpos.
        public PosRepository(
            //IConfiguration _configuration,
            POSConnection posConnect,
            PTMaxstationContext context,
            IUnitOfWork unitOfWork,
            HttpClient client
            ) : base(context)
        {
            //  _connectionString = _configuration.GetConnectionString("OracleDbConnection");

            //posConnect.ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=172.16.112.207)(PORT=1524))(CONNECT_DATA=(SERVICE_NAME=orapos)));Persist Security Info=True;User Id=raptorpos;Password=raptor18;";
            _connectionString = posConnect.ConnectionString;
            _client = client;
            //_connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=172.16.112.207)(PORT=1524))(CONNECT_DATA=(SERVICE_NAME=orapos)));Persist Security Info=True;User Id=raptorpos;Password=raptor18;";
        }

        public string GetConn()
        {
            return _connectionString;
        }

        public QueryResult<POSCash> CashList(CashQueryResource query)
        {
            try
            {
                List<POSCash> cashList = new List<POSCash>();

                var cashPayGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Cash");
                var disCountPayGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Discount");
                var couponPayGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Coupon");
                var policeGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Police");
                var payTypes = context.MasPayTypes.Where(x => x.PayGroupId == cashPayGroup.PayGroupId || x.PayGroupId == disCountPayGroup.PayGroupId || x.PayGroupId == couponPayGroup.PayGroupId);
                var payGroupIds = payTypes.Select(x => x.PayTypeId);
                var payTypeIds = string.Join(",", payGroupIds);

                var payTypePolices = policeGroup == null ? "0" : string.Join(",", context.MasPayTypes.Where(x => x.PayGroupId == policeGroup.PayGroupId).Select(x => x.PayTypeId).ToList());
                //var journalIds = string.Format("'{0}'", string.Join("','", cashJournalIds.Select(i => i.Replace("'", "''"))));
                //string shiftNos = string.Join(",", shiftNoList.Select(x => string.Format("'{0}'", x)));

                OracleConnection con = new OracleConnection
                {
                    ConnectionString = _connectionString
                };
                con.Open();

                OracleCommand cmd = new OracleCommand
                {
                    Connection = con,
                    CommandTimeout = 60,
                    CommandText = $@"SELECT DISTINCT SHIFT_NO FROM {schema}FUNCTION2 WHERE SITE_ID = '52{query.BrnCode}' AND TRUNC(FUNCTION2.BUSINESS_DATE) = TO_DATE('{query.FromDate:dd/MM/yyyy}','dd/MM/yyyy') "
                    //CommandText = $@"SELECT DISTINCT SHIFT_NO FROM {schema}BILL_HEADER WHERE BRANCH_CODE = '52{query.BrnCode}' AND TRUNC(BUSINESS_DATE) = TO_DATE('{query.FromDate:dd/MM/yyyy}','dd/MM/yyyy') "
                };

                OracleDataReader oracleDataReader = cmd.ExecuteReader();

                var shiftNoList = new List<string>();

                while (oracleDataReader.Read())
                {
                    var shiftNo = oracleDataReader["SHIFT_NO"].ToString();
                    shiftNoList.Add(shiftNo);
                }

                oracleDataReader.Close();
                oracleDataReader.Dispose();
                con.Close();

                var posLogs = context.DopPos.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.LocCode == query.LocCode && x.DocDate.Date == query.FromDate.Date && x.PayGroupId == cashPayGroup.PayGroupId);

                if (posLogs.Any())
                {
                    foreach (var poslog in posLogs)
                    {
                        shiftNoList.Remove(poslog.Period.ToString());
                    }
                }


                if (shiftNoList.Any())
                {
                    foreach (var shiftNo in shiftNoList)
                    {
                        var posCashsSql = GetPOSCashesFromSqlServer(query.FromDate, query.BrnCode, payTypeIds, shiftNo, payTypePolices);

                        if (posCashsSql.Count > 0)
                        {
                            cashList.AddRange(posCashsSql);
                        }
                        else
                        {
                            var posCashsOracle = GetPOSCashesFromOracle(query.FromDate, query.BrnCode, payTypeIds, shiftNo, payTypePolices);
                            cashList.AddRange(posCashsOracle);
                        }
                    }
                }

                oracleDataReader.Close();
                oracleDataReader.Dispose();
                con.Close();

                con.Dispose();
                con = null;
                GC.Collect();

                int intSeqNo = 1;
                foreach (var cash in cashList)
                {
                    cash.Row = intSeqNo++;
                }

                return new QueryResult<POSCash>
                {
                    Items = cashList,
                    TotalItems = cashList.Count
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public QueryResult<POSCredit> CreditList(CreditQueryResource query)
        {
            try
            {
                List<POSCredit> creditList = new List<POSCredit>();

                var creditPayGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Credit");
                var creditPayType = context.MasPayTypes.Where(x => x.PayGroupId == creditPayGroup.PayGroupId);
                var creditPayGroupIds = creditPayType.Select(x => x.PayTypeId);
                var creditPayTypeIds = string.Join(",", creditPayGroupIds);

                OracleConnection con = new OracleConnection
                {
                    ConnectionString = _connectionString
                };
                con.Open();

                OracleCommand cmd = new OracleCommand
                {
                    Connection = con,
                    CommandTimeout = 60,
                    CommandText = $@"SELECT DISTINCT SHIFT_NO FROM {schema}FUNCTION2 WHERE SITE_ID = '52{query.BrnCode}' AND TRUNC(FUNCTION2.BUSINESS_DATE) = TO_DATE('{query.FromDate:dd/MM/yyyy}','dd/MM/yyyy') "
                    //CommandText = $@"SELECT DISTINCT SHIFT_NO FROM {schema}BILL_HEADER WHERE BRANCH_CODE = '52{query.BrnCode}' AND TRUNC(BUSINESS_DATE) = TO_DATE('{query.FromDate:dd/MM/yyyy}','dd/MM/yyyy') "

                };

                OracleDataReader oracleDataReader = cmd.ExecuteReader();

                var shiftNoList = new List<string>();

                while (oracleDataReader.Read())
                {
                    var shiftNo = oracleDataReader["SHIFT_NO"].ToString();
                    shiftNoList.Add(shiftNo);
                }

                oracleDataReader.Close();
                oracleDataReader.Dispose();
                con.Close();

                var posLogs = context.DopPos.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.LocCode == query.LocCode && x.DocDate.Date == query.FromDate.Date && x.PayGroupId == creditPayGroup.PayGroupId);

                if (posLogs.Any())
                {
                    foreach (var poslog in posLogs)
                    {
                        shiftNoList.Remove(poslog.Period.ToString());
                    }
                }

                if (shiftNoList.Any())
                {
                    foreach (var shiftNo in shiftNoList)
                    {
                        var posCreditsSql = GetPOSCreditsFromSqlServer(query.FromDate, query.BrnCode, creditPayTypeIds, shiftNo);

                        if (posCreditsSql.Count > 0)
                        {
                            creditList.AddRange(posCreditsSql);
                        }
                        else
                        {
                            var posCreditsOracle = GetPOSCreditsFromOracle(query.FromDate, query.BrnCode, creditPayTypeIds, shiftNo);
                            creditList.AddRange(posCreditsOracle);
                        }
                    }
                }

                oracleDataReader.Close();
                oracleDataReader.Dispose();
                con.Close();
                con.Dispose();
                con = null;
                GC.Collect();

                int intSeqNo = 1;
                foreach (var credit in creditList)
                {
                    credit.Row = intSeqNo++;
                }

                return new QueryResult<POSCredit>
                {
                    Items = creditList,
                    ItemsPerPage = query.ItemsPerPage,
                    TotalItems = creditList.Count
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public QueryResult<POSWithdraw> WithdrawList(WithdrawQueryResource query)
        {
            try
            {
                List<POSWithdraw> WithdrawList = new List<POSWithdraw>();

                var withdrawPayGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Withdraw");
                var withdrawPayType = context.MasPayTypes.Where(x => x.PayGroupId == withdrawPayGroup.PayGroupId);
                var withdrawPayGroupIds = withdrawPayType.Select(x => x.PayTypeId);
                var withdrawPayTypeIds = string.Join(",", withdrawPayGroupIds);
                var waterPosConfig = context.DopPosConfigs.FirstOrDefault(x => x.DocType == "Withdraw" && x.DocDesc == "Water");

                OracleConnection con = new OracleConnection
                {
                    ConnectionString = _connectionString
                };
                con.Open();
                OracleCommand cmd = new OracleCommand
                {
                    Connection = con,
                    CommandTimeout = 60,
                    CommandText = $@"SELECT DISTINCT SHIFT_NO FROM {schema}FUNCTION2 WHERE SITE_ID = '52{query.BrnCode}' AND TRUNC(FUNCTION2.BUSINESS_DATE) = TO_DATE('{query.FromDate:dd/MM/yyyy}','dd/MM/yyyy') "

                };

                OracleDataReader oracleDataReader = cmd.ExecuteReader();

                var shiftNoList = new List<string>();

                while (oracleDataReader.Read())
                {
                    var shiftNo = oracleDataReader["SHIFT_NO"].ToString();
                    shiftNoList.Add(shiftNo);
                }

                oracleDataReader.Close();
                oracleDataReader.Dispose();
                con.Close();

                var posLogs = context.DopPos.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.LocCode == query.LocCode && x.DocDate.Date == query.FromDate.Date && x.PayGroupId == withdrawPayGroup.PayGroupId);

                if (posLogs.Any())
                {
                    foreach (var poslog in posLogs)
                    {
                        shiftNoList.Remove(poslog.Period.ToString());
                    }
                }

                if (shiftNoList.Any())
                {
                    foreach (var shiftNo in shiftNoList)
                    {
                        var posWithdrawSql = GetPOSWithdrawFromSqlServer(query.FromDate, query.BrnCode, withdrawPayTypeIds, shiftNo, waterPosConfig.PdId, query.CompCode);

                        if (posWithdrawSql.Count > 0)
                        {
                            WithdrawList.AddRange(posWithdrawSql);
                        }
                        else
                        {
                            var posWithdrawOracle = GetPOSWithdrawFromOracle(query.FromDate, query.BrnCode, withdrawPayTypeIds, shiftNo, waterPosConfig.PdId, query.CompCode);
                            WithdrawList.AddRange(posWithdrawOracle);
                        }
                    }
                }

                oracleDataReader.Close();
                oracleDataReader.Dispose();
                con.Close();
                con.Dispose();
                con = null;
                GC.Collect();

                WithdrawList = WithdrawList.OrderBy(x => x.ShiftNo).ThenBy(x => x.PluNumber).ToList();

                //var branchPeriods = GetPOSWorktimeFromOracle(query.FromDate, query.BrnCode);
                var waterWithDraws = WithdrawList.Where(x => x.PluNumber == waterPosConfig.PdId);
                if (waterWithDraws != null && waterWithDraws.Count() > 0 && waterPosConfig.DocStatus.Equals("Active"))
                {
                    var branchMids = context.MasBranchMids.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode).ToList();

                    foreach (var waterWithdraw in waterWithDraws)
                    {
                        var sumWater = 0;
                        //var branchPeriods = context.MasBranchPeriods.FirstOrDefault(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.PeriodNo == Convert.ToInt32(waterWithdraw.ShiftNo));                        
                        //var worktime = waterWithdraw.Where(x => x.ShiftNo == waterWithdraw.ShiftNo).FirstOrDefault();
                        foreach (var branchmid in branchMids)
                        {
                            //var startDate = worktime.StartTime.ToString("yyyyMMddHHmmss"); //query.FromDate.ToString("yyyyMMdd") + worktime.StartTime.Replace(@".", string.Empty) + "00";
                            //var endDate = worktime.EndTime.ToString("yyyyMMddHHmmss");  //query.FromDate.ToString("yyyyMMdd") + worktime.TimeFinish.Replace(@".", string.Empty) + "00";
                            var startDate = waterWithdraw.StartTime.ToString("yyyyMMddHHmmss");
                            var endDate = waterWithdraw.EndTime.ToString("yyyyMMddHHmmss");
                            var giveWater = GetProfileByPass(branchmid.MidNo, startDate, endDate, waterPosConfig.ApiUrl).Result.data;

                            if (giveWater != null)
                            {
                                sumWater += Convert.ToInt32(giveWater.SumWater);
                            }
                        }

                        waterWithdraw.SumWater = sumWater;
                    }

                }

                var periodGroup = WithdrawList.GroupBy(x => x.ShiftNo).Select(group => group.First());
                var periods = periodGroup.Select(x => x.ShiftNo).ToList();
                //var posLogs = context.DopPos.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.LocCode == query.LocCode && x.DocDate.Date == query.FromDate.Date && x.PayGroupId == withdrawPayGroup.PayGroupId);

                foreach (var period in periods)
                {
                    var cashPeriod = WithdrawList.Where(x => x.ShiftNo == period);

                    var posLogPeriod = posLogs.FirstOrDefault(x => x.Period == Convert.ToInt32(period));

                    if (posLogPeriod != null)
                    {
                        var posLogJournalIds = JsonSerializer.Deserialize<List<string>>(posLogPeriod.JsonData);
                        WithdrawList = WithdrawList.Where(x => !posLogJournalIds.Contains(x.JournalId)).ToList();
                    }
                }

                int intSeqNo = 1;
                foreach (var withdraw in WithdrawList)
                {
                    withdraw.Row = intSeqNo++;
                }

                return new QueryResult<POSWithdraw>
                {
                    Items = WithdrawList.OrderBy(x => x.ShiftNo).ThenBy(x => x.PluNumber).ToList(),
                    ItemsPerPage = query.ItemsPerPage
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public QueryResult<POSReceive> ReceiveList(ReceiveQueryResource query)
        {
            try
            {
                List<POSReceive> receiveList = new List<POSReceive>();

                var cashGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Cash");
                var receiveGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Receive");
                var disCountPayGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Discount");
                var couponPayGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Coupon");
                var policeGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Police");
                var payTypes = context.MasPayTypes.Where(x => x.PayGroupId == cashGroup.PayGroupId || x.PayGroupId == disCountPayGroup.PayGroupId || x.PayGroupId == couponPayGroup.PayGroupId).ToList();
                var payGroupIds = payTypes.Select(x => x.PayTypeId);
                var payTypeIds = string.Join(",", payGroupIds);
                var payTypePolices = policeGroup == null ? "0" : string.Join(",", context.MasPayTypes.Where(x => x.PayGroupId == policeGroup.PayGroupId).Select(x => x.PayTypeId).ToList());

                OracleConnection con = new OracleConnection
                {
                    ConnectionString = _connectionString
                };
                con.Open();

                OracleCommand cmd = new OracleCommand
                {
                    Connection = con,
                    CommandTimeout = 60,
                    CommandText = $@"SELECT DISTINCT SHIFT_NO FROM {schema}FUNCTION2 WHERE SITE_ID = '52{query.BrnCode}' AND TRUNC(FUNCTION2.BUSINESS_DATE) = TO_DATE('{query.FromDate:dd/MM/yyyy}','dd/MM/yyyy') "
                    //CommandText = $@"SELECT DISTINCT SHIFT_NO FROM {schema}BILL_HEADER WHERE BRANCH_CODE = '52{query.BrnCode}' AND TRUNC(BUSINESS_DATE) = TO_DATE('{query.FromDate:dd/MM/yyyy}','dd/MM/yyyy') "

                };

                OracleDataReader oracleDataReader = cmd.ExecuteReader();

                var shiftNoList = new List<string>();

                while (oracleDataReader.Read())
                {
                    var shiftNo = oracleDataReader["SHIFT_NO"].ToString();
                    shiftNoList.Add(shiftNo);
                }

                oracleDataReader.Close();
                oracleDataReader.Dispose();
                con.Close();

                var posLogs = context.DopPos.Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.LocCode == query.LocCode && x.DocDate.Date == query.FromDate.Date && x.PayGroupId == receiveGroup.PayGroupId);

                if (posLogs.Any())
                {
                    foreach (var poslog in posLogs)
                    {
                        shiftNoList.Remove(poslog.Period.ToString());
                    }
                }


                if (shiftNoList.Any())
                {
                    foreach (var shiftNo in shiftNoList)
                    {
                        var posReceivesSql = GetPOSReceivesFromSqlServer(query.FromDate, query.BrnCode, payTypeIds, shiftNo, payTypePolices);

                        if (posReceivesSql.Count > 0)
                        {
                            receiveList.AddRange(posReceivesSql);
                        }
                        else
                        {
                            posReceivesSql = GetPOSReceivesFromOracle(query.FromDate, query.BrnCode, payTypeIds, shiftNo, payTypePolices);
                            receiveList.AddRange(posReceivesSql);
                        }
                    }
                }

                oracleDataReader.Close();
                oracleDataReader.Dispose();
                con.Close();

                con.Dispose();
                con = null;
                GC.Collect();

                int intSeqNo = 1;
                foreach (var receive in receiveList)
                {
                    receive.Row = intSeqNo++;
                }

                return new QueryResult<POSReceive>
                {
                    Items = receiveList,
                    TotalItems = receiveList.Count
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private List<POSWithdraw> GetPOSWithdrawFromSqlServer(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo, string water, string compCode)
        {
            var posWithdraws = new List<POSWithdraw>();
            //string strSql = $@"select  tbl.site_id,tbl.business_date,tbl.shift_no,tbl.plu_number,sum(tbl.sell_qty) as sell_qty,MIN(tbl.CREATED_DATE)as start_time,max(tbl.CREATED_DATE)as end_time
            //                from (	                            
            //                 select f4.journal_id, f4.site_id, f4.business_date, f4.shift_no, f5.plu_number, f5.sell_qty,f4.CREATED_DATE
            //                 from INF_POS_FUNCTION4(nolock) f4
            //                 inner join INF_POS_FUNCTION5(nolock) f5
            //                 on f4.journal_id = f5.journal_id
            //                 where f5.disc_group <> 'F'
            //                 and convert(date,f4.business_date) = '{fromDate:yyyy-MM-dd}' --parameter: วันที่โหลดข้อมูล
            //                 and f4.site_id = '52{brnCode}' --parameter: branch
            //                 AND f4.shift_no = {shiftNo} 	 --parameter : period_no     
            //                 and EXISTS ( select null 
            //                  from INF_POS_FUNCTION14(nolock) f14
            //                  where f14.journal_id = f4.journal_id
            //                  and f14.site_id = f4.site_id 		
            //                  and f14.mop_code in ({payTypeIds}) --parameter : pay_type_id     
            //                  )	
            //                 union all	                            
            //                 select   f4.journal_id, f4.site_id, f4.business_date, f4.shift_no , f5.plu_number, f5.sell_qty,f4.CREATED_DATE
            //                 from INF_POS_FUNCTION4(nolock) f4
            //                 inner join INF_POS_FUNCTION5(nolock) f5
            //                 on  f4.journal_id = f5.journal_id 
            //                 where  f5.disc_group = 'F'
            //                 and convert(date,f4.business_date) = '{fromDate:yyyy-MM-dd}' --parameter: วันที่โหลดข้อมูล
            //                 and f4.site_id =  '52{brnCode}' -- branch
            //                 AND f4.shift_no =  {shiftNo} --parameter : period_no
            //                 and f5.plu_number in ('{water}') -- pd_id  

            //                ) tbl
            //                group by tbl.site_id,tbl.business_date,tbl.shift_no,tbl.plu_number";

            string strSql = $@"select  tbl.site_id,tbl.business_date,tbl.shift_no,tbl.plu_number,isnull(tbl.REF_COSTCENTER,'') REF_COSTCENTER,isnull(tbl.LICENSENO,'') LICENSENO
	                        ,sum(tbl.sell_qty) as sell_qty,MIN(tbl.CREATED_DATE)as start_time,max(tbl.CREATED_DATE)as end_time
                            from (	                            
                             select f4.journal_id, f4.site_id, f4.business_date, f4.shift_no, f5.plu_number, f5.sell_qty,f4.CREATED_DATE
                            ,pe.REF_COSTCENTER,pe.LICENSENO
                             from INF_POS_FUNCTION4(nolock) f4
                             inner join INF_POS_FUNCTION5(nolock) f5
                             on f4.journal_id = f5.journal_id
		                    left join INF_POS_ENTERPRISE pe
		                    on f4.SITE_ID = pe.SITE_ID
		                    and f4.SHIFT_NO = pe.SHIFT_NO
		                    and f4.JOURNAL_ID = pe.JOURNAL_ID
		                    and pe.PRODUCT_CODE <> 'N' 
                             where f5.disc_group <> 'F'
                             and convert(date,f4.business_date) = '{fromDate:yyyy-MM-dd}' --parameter: วันที่โหลดข้อมูล
                             and f4.site_id = '52{brnCode}' --parameter: branch
                             AND f4.shift_no = {shiftNo} 	 --parameter : period_no     
                             and EXISTS ( select null 
                              from INF_POS_FUNCTION14(nolock) f14
                              where f14.journal_id = f4.journal_id
                              and f14.site_id = f4.site_id 		
                              and f14.mop_code in ({payTypeIds}) --parameter : pay_type_id     
                              )	
                             union all	                            
                             select   f4.journal_id, f4.site_id, f4.business_date, f4.shift_no , f5.plu_number, f5.sell_qty,f4.CREATED_DATE,'',''
                             from INF_POS_FUNCTION4(nolock) f4
                             inner join INF_POS_FUNCTION5(nolock) f5
                             on  f4.journal_id = f5.journal_id 
                             where  f5.disc_group = 'F'
                             and convert(date,f4.business_date) = '{fromDate:yyyy-MM-dd}' --parameter: วันที่โหลดข้อมูล
                             and f4.site_id =  '52{brnCode}' -- branch
                             AND f4.shift_no =  {shiftNo} --parameter : period_no
                             and f5.plu_number in ('{water}') -- pd_id  

                            ) tbl
                            group by tbl.site_id,tbl.business_date,tbl.shift_no,tbl.plu_number,tbl.REF_COSTCENTER,tbl.LICENSENO";

            using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = strSql;
            context.Database.OpenConnection();

            var products = context.MasProducts.ToList();
            var masCompanyCars = context.MasCompanyCars.ToList();
            var masCosCenters = context.MasCostCenters.ToList();

            using (System.Data.Common.DbDataReader result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    var pluNumber = result["PLU_NUMBER"].ToString().Trim();
                    var licenseNo = result["LICENSENO"].ToString().Trim();
                    var refCoscenter = result["REF_COSTCENTER"].ToString().Trim();
                    var masproduct = products.FirstOrDefault(x => x.PdId == pluNumber); //context.MasProducts.FirstOrDefault(x => x.PdId == pluNumber);
                    var licensePlate = string.Empty;
                    var cosCenter = string.Empty;

                    if (licenseNo != "")
                    {
                        var masCompanyCar = masCompanyCars.FirstOrDefault(x => x.CompCode == compCode && x.LicensePlate.Trim() == licenseNo);
                        licensePlate = masCompanyCar != null ? masCompanyCar.LicensePlate.Trim() : string.Empty;
                    }

                    if (refCoscenter != "")
                    {
                        var masCosCenter = masCosCenters.FirstOrDefault(x => x.CompCode == compCode && x.CostCenter == refCoscenter);
                        cosCenter = masCosCenter != null ? masCosCenter.BrnCode : string.Empty;
                    }

                    POSWithdraw wth = new POSWithdraw
                    {
                        JournalId = result["PLU_NUMBER"].ToString(),
                        SiteId = result["SITE_ID"].ToString(),
                        BusinessDate = Convert.ToDateTime(result["BUSINESS_DATE"]),
                        ShiftNo = result["SHIFT_NO"].ToString(),
                        PluNumber = result["PLU_NUMBER"].ToString(),
                        //CostCenter = result["REF_COSTCENTER"].ToString(),
                        //LicensePlate = result["LICENSENO"].ToString(),
                        SellQty = Convert.ToDecimal(result["SELL_QTY"]),
                        StartTime = Convert.ToDateTime(result["start_time"]),
                        EndTime = Convert.ToDateTime(result["end_time"]),
                        ItemCode = masproduct != null ? masproduct.PdId : string.Empty,
                        ItemName = masproduct != null ? masproduct.PdName : string.Empty,
                        LicensePlate = licensePlate,
                        CostCenter = cosCenter
                    };

                    posWithdraws.Add(wth);
                }

            }
            context.Database.CloseConnection();

            return posWithdraws;
        }


        private List<POSWithdraw> GetPOSWithdrawFromOracle(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo, string water, string compCode)
        {

            var posWithdraws = new List<POSWithdraw>();
            var commandText = @$"select  tbl.site_id,tbl.business_date,tbl.shift_no,tbl.plu_number,tbl.REF_COSTCENTER,tbl.LICENSENO
                        ,sum(tbl.sell_qty) as sell_qty,min(tbl.createdate) AS start_time,max(tbl.createdate) AS end_time
                        from (
                        select  bh.bill_no journal_id, bh.branch_at AS site_id,trunc(bh.business_date)AS business_date ,bh.SHIFT_NO ,prod.product_code plu_number  ,  bd.sell_qty sell_qty,bh.CREATEDATE
                        , case when esb.REF_COSTCENTER is null then to_nchar('') else esb.REF_COSTCENTER end as REF_COSTCENTER
                        , case when esb.LICENSENO is null then to_nchar('') else esb.LICENSENO end as LICENSENO
                        from {schema}bill_header bh, {schema}bill_detail bd,{schema}payment_detail pd, {schema}product prod
                        ,{schema}ENTERPRISE_SOLUTION_BILL esb
                        where bh.com_code = bd.com_code
                            and bh.branch_code = bd.branch_code
                            and bh.pos_id = bd.pos_id
                            and bh.pos_day_id = bd.pos_day_id
                            and bh.shift_no = bd.shift_no
                            and bh.bill_id = bd.bill_id
                            and bd.ref_t_product_code = prod.product_code
                            AND pd.com_code = bh.com_code
                            AND pd.branch_code = bh.branch_code
                            AND pd.pos_id = bh.pos_id
                            AND pd.pos_day_id = bh.pos_day_id
                            AND pd.shift_no = bh.shift_no          
                            and bh.bill_status <> '0002'
                            AND bh.branch_at = esb.branch_code(+) 
                            AND bh.SHIFT_NO = esb.SHIFT_NO (+) 
                            AND trunc(bh.business_date) = TRUNC(esb.SALE_DATE (+) )
                            AND bh.BILL_NO = ESB .BILL_NO (+)   
                            AND EXISTS
                                    (SELECT NULL
                                    FROM raptorpos.payment_header ph
                                    WHERE     bh.com_code = ph.com_code
                                            AND bh.branch_code = ph.branch_code
                                            AND bh.pos_id = ph.pos_id
                                            AND bh.pos_day_id = ph.pos_day_id
                                            AND bh.shift_no = ph.shift_no
                                            AND bh.sale_id = ph.sale_id
                                            AND pd.payment_id = ph.payment_id)    
                            and prod.product_type <> 'F'
                            and trunc(bh.BUSINESS_DATE) =  to_date('{fromDate:yyyy-MM-dd}','yyyy-mm-dd')
                            AND bh.branch_at = '52{brnCode}'
                            AND bh.shift_no = {shiftNo}  -->parameter period
                            and pd.pay_types in ({payTypeIds})
                            UNION ALL   
                            select  bh.bill_no journal_id, bh.branch_at AS site_id,trunc(bh.business_date)AS business_date ,bh.SHIFT_NO ,prod.product_code plu_number  ,  bd.sell_qty sell_qty,bh.CREATEDATE
                            ,to_nchar('') AS REF_COSTCENTER, to_nchar('') AS  LICENSENO
                        from raptorpos.bill_header bh, raptorpos.bill_detail bd, raptorpos.product prod
                        , raptorpos.s_transaction_detail trd
                        where bh.com_code = bd.com_code
                            and bh.branch_code = bd.branch_code
                            and bh.pos_id = bd.pos_id
                            and bh.pos_day_id = bd.pos_day_id
                            and bh.shift_no = bd.shift_no
                            and bh.bill_id = bd.bill_id
                            and bd.ref_t_product_code = prod.product_code    
                            and bh.com_code = trd.com_code
                            and bh.branch_code = trd.branch_code
                            and bh.pos_id = trd.pos_id
                            and bh.pos_day_id = trd.pos_day_id
                            and bh.shift_no = trd.shift_no
                            and bh.sale_id = trd.sale_id
                            and bd.branch_code = trd.branch_code
                            and bd.pos_id = trd.pos_id
                            and bd.pos_day_id = trd.pos_day_id
                            and bd.shift_no = trd.shift_no
                            and bd.ref_t_trans_id = trd.ref_t_trans_id
                            and bd.item_id = trd.sale_item_id  
                            and trd.itemtype in ('O', 'N', 'F')
                            and bh.bill_status <> '0002'
                            and prod.product_type = 'F' 
                            and trunc(bh.BUSINESS_DATE) =  to_date('{fromDate:yyyy-MM-dd}','yyyy-mm-dd') -->parameter doc_date
                            AND bh.branch_at = '52{brnCode}'		-->parameter brn_code
                            AND bh.shift_no = {shiftNo}	-->parameter period
                            and prod.PRODUCT_CODE  in ('{water}') --> parameter pd_id  
                        )tbl
                        group by tbl.site_id,tbl.business_date,tbl.shift_no,tbl.plu_number,tbl.REF_COSTCENTER,tbl.LICENSENO";

            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 60,
                CommandText = commandText
            };
            var produsts = this.context.MasProducts.ToList();
            var masCompanyCars = this.context.MasCompanyCars.ToList();
            var masCosCenters = this.context.MasCostCenters.ToList();

            using (System.Data.Common.DbDataReader result = cmd.ExecuteReader())
            {
                while (result.Read())
                {
                    var pluNumber = result["PLU_NUMBER"].ToString().Trim();
                    var licenseNo = result["LICENSENO"].ToString().Trim();
                    var refCoscenter = result["REF_COSTCENTER"].ToString().Trim();
                    var masproduct = produsts.FirstOrDefault(x => x.PdId == pluNumber);
                    var licensePlate = string.Empty;
                    var cosCenter = string.Empty;

                    if (licenseNo != "")
                    {
                        var masCompanyCar = masCompanyCars.FirstOrDefault(x => x.CompCode == compCode && x.LicensePlate.Trim() == licenseNo);
                        licensePlate = masCompanyCar != null ? masCompanyCar.LicensePlate.Trim() : string.Empty;
                    }

                    if (refCoscenter != "")
                    {
                        var masCosCenter = masCosCenters.FirstOrDefault(x => x.CompCode == compCode && x.CostCenter == refCoscenter);
                        cosCenter = masCosCenter != null ? masCosCenter.BrnCode : string.Empty;
                    }


                    POSWithdraw wth = new POSWithdraw
                    {
                        JournalId = result["PLU_NUMBER"].ToString(),
                        SiteId = result["SITE_ID"].ToString(),
                        BusinessDate = Convert.ToDateTime(result["BUSINESS_DATE"]),
                        ShiftNo = result["SHIFT_NO"].ToString(),
                        PluNumber = result["PLU_NUMBER"].ToString(),
                        //CostCenter = result["REF_COSTCENTER"].ToString(),
                        //LicensePlate = result["LICENSENO"].ToString(),
                        SellQty = Convert.ToDecimal(result["SELL_QTY"]),
                        StartTime = Convert.ToDateTime(result["start_time"]),
                        EndTime = Convert.ToDateTime(result["end_time"]),
                        ItemCode = masproduct != null ? masproduct.PdId : string.Empty,
                        ItemName = masproduct != null ? masproduct.PdName : string.Empty,
                        LicensePlate = licensePlate,
                        CostCenter = cosCenter
                    };

                    posWithdraws.Add(wth);
                }

            }

            con.Close();

            return posWithdraws;
        }


        private List<POSWorktime> GetPOSWorktimeFromOracle(DateTime fromDate, string brnCode)
        {

            var posWorktimes = new List<POSWorktime>();
            var commandText = @$"SELECT bh.shift_no,min(bh.createdate)  AS start_time,max(bh.createdate)  AS end_time
                                FROM {schema}bill_header bh, {schema}payment_detail pd                                
                                where pd.com_code = bh.com_code
                                  AND pd.branch_code = bh.branch_code
                                  AND pd.pos_id = bh.pos_id
                                  AND pd.pos_day_id = bh.pos_day_id
                                  AND pd.shift_no = bh.shift_no          
                                and  bh.bill_status <> '0002'
                                and bh.BRANCH_CODE  = '52{brnCode}'	--> PARAMETER brn_code   
                                AND trunc(bh.BUSINESS_DATE) =   to_date('{fromDate:yyyy-MM-dd}','yyyy-mm-dd')	--> PARAMETER doc_date
                                AND EXISTS
                                 (SELECT NULL
                                    FROM {schema}payment_header ph
                                   WHERE  bh.com_code = ph.com_code
                                         AND bh.branch_code = ph.branch_code
                                         AND bh.pos_id = ph.pos_id
                                         AND bh.pos_day_id = ph.pos_day_id
                                         AND bh.shift_no = ph.shift_no
                                         AND bh.sale_id = ph.sale_id
                                         AND pd.payment_id = ph.payment_id)
                                GROUP BY bh.shift_no";

            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 60,
                CommandText = commandText
            };

            using (System.Data.Common.DbDataReader result = cmd.ExecuteReader())
            {
                while (result.Read())
                {
                    var posWorktime = new POSWorktime()
                    {
                        ShiftNo = result["shift_no"].ToString(),
                        StartTime = Convert.ToDateTime(result["start_time"]),
                        EndTime = Convert.ToDateTime(result["end_time"])
                    };
                    posWorktimes.Add(posWorktime);

                }

            }

            con.Close();
            return posWorktimes;
        }




        public async Task AddCashSaleListAsync(SaveCashSaleResource request)
        {
            var cashsaleGroups = request._Cashsale
                .Select(x =>
                new
                {
                    x.JournalId,
                    x.SiteId,
                    x.BusinessDate,
                    x.ShiftNo,
                    x.TaxInvNo,
                    x.TotalGoodsAmt,
                    x.TotalDiscAmt,
                    x.TotalTaxAmt,
                    x.TotalPaidAmt,
                    x.PluNumber,
                    x.SelQty,
                    x.SalePrice,
                    x.GoodsAmt,
                    x.TaxAmt,
                    x.DiscAmt,
                    x.BillNo,
                    x.ItemCode,
                    x.ItemName,
                    x.SubAmount,
                    x.TotalAmount,
                    x.EmpCode
                })
                .GroupBy(s => new { s.JournalId, s.SiteId, s.BusinessDate, s.ShiftNo, s.TaxInvNo, s.EmpCode })
                .Select(g =>
                    new
                    {
                        g.Key.JournalId,
                        g.Key.SiteId,
                        g.Key.BusinessDate,
                        g.Key.ShiftNo,
                        g.Key.TaxInvNo,
                        g.Key.EmpCode,
                        TotalGoodsAmt = g.Sum(x => x.TotalGoodsAmt),
                        TotalDiscAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.TotalDiscAmt), 2)),
                        TotalTaxAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.TotalTaxAmt), 2)),
                        TotalPaidAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.TotalPaidAmt), 2)),
                        SelQty = g.Sum(x => Math.Round(Convert.ToDecimal(x.SelQty), 2)),
                        SalePrice = g.Sum(x => Math.Round(Convert.ToDecimal(x.SalePrice), 2)),
                        GoodsAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.GoodsAmt), 2)),
                        TaxAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.TaxAmt), 2)),
                        DiscAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.DiscAmt), 2)),
                        SubAmount = g.Sum(x => Math.Round(Convert.ToDecimal(x.SubAmount), 2)),
                        TotalAmount = g.Sum(x => Math.Round(Convert.ToDecimal(x.TotalAmount), 2)),
                    });

            var docPattern = context.MasDocPatterns.FirstOrDefault(x => x.DocType == "CashSale");
            var pattern = (docPattern == null) ? string.Empty : docPattern.Pattern;

            pattern = pattern.Replace("Brn", request.BrnCode);
            pattern = pattern.Replace("yy", request.SystemDate.ToString("yy"));
            pattern = pattern.Replace("MM", request.SystemDate.ToString("MM"));

            var runno = GetCashRunNumber(request.CompCode, request.BrnCode, pattern);
            var journalIdList = context.SalCashsaleHds.Where(x => x.CompCode == request.CompCode && x.BrnCode == request.BrnCode && x.LocCode == request.LocCode && x.DocDate == request.SystemDate).ToList();
            //var masEmployees = context.MasEmployees.ToList();

            foreach (var cashsaleGroup in cashsaleGroups)
            {
                //var employee = masEmployees.FirstOrDefault(x => x.EmpCode == cashsaleGroup.EmpCode.Trim());
                var cashsaleHd = new SalCashsaleHd();
                //do
                //{
                //    cashsaleHd.DocNo = GenDocNo(request.SystemDate, request.BrnCode, pattern, ++runno, "CashSale");
                //} while (await IsDuplicateCashSale(request , cashsaleHd.DocNo));
                //cashsaleHd.RunNumber = runno;
                cashsaleHd.RunNumber = ++runno;
                cashsaleHd.CompCode = request.CompCode;
                cashsaleHd.BrnCode = request.BrnCode;
                cashsaleHd.LocCode = request.LocCode;
                cashsaleHd.DocStatus = "Active";
                cashsaleHd.DocDate = request.SystemDate;
                cashsaleHd.DocNo = GenDocNo(request.SystemDate, request.BrnCode, pattern, (int)cashsaleHd.RunNumber, "CashSale");
                cashsaleHd.RefNo = cashsaleGroup.JournalId;
                cashsaleHd.ItemCount = 0;
                cashsaleHd.Currency = "THB";
                cashsaleHd.CurRate = 1m;
                cashsaleHd.SubAmt = cashsaleGroup.SubAmount; //cashsaleGroup.GoodsAmt + cashsaleGroup.TaxAmt;
                cashsaleHd.SubAmtCur = cashsaleGroup.SubAmount; //cashsaleGroup.GoodsAmt + cashsaleGroup.TaxAmt;
                cashsaleHd.DiscRate = string.Empty;
                cashsaleHd.DiscAmt = 0;
                cashsaleHd.DiscAmtCur = 0;
                cashsaleHd.TotalAmt = cashsaleGroup.TotalAmount; //cashsaleGroup.GoodsAmt + cashsaleGroup.TaxAmt;
                cashsaleHd.TotalAmtCur = cashsaleGroup.TotalAmount; //cashsaleGroup.GoodsAmt + cashsaleGroup.TaxAmt;
                cashsaleHd.TaxBaseAmt = cashsaleGroup.GoodsAmt;
                cashsaleHd.TaxBaseAmtCur = cashsaleGroup.GoodsAmt;
                cashsaleHd.VatRate = 0;
                cashsaleHd.VatAmt = cashsaleGroup.TaxAmt;
                cashsaleHd.VatAmtCur = cashsaleGroup.TaxAmt;
                cashsaleHd.NetAmt = cashsaleGroup.GoodsAmt + cashsaleGroup.TaxAmt;
                cashsaleHd.NetAmtCur = cashsaleGroup.GoodsAmt + cashsaleGroup.TaxAmt;
                cashsaleHd.DocPattern = pattern;
                cashsaleHd.Post = "N";
                cashsaleHd.CreatedDate = DateTime.Now;
                cashsaleHd.CreatedBy = request.CreatedBy;
                cashsaleHd.UpdatedDate = DateTime.Now;
                //cashsaleHd.EmpCode = employee != null ? employee.EmpCode : string.Empty;
                //cashsaleHd.EmpName = employee != null ? employee.EmpName : string.Empty;

                if (journalIdList.Count > 0)
                {
                    // var is_exists = journalIdList.Any(x => x.RefNo == cashsaleHd.RefNo);
                    if (journalIdList.Any(x => x.RefNo == cashsaleHd.RefNo))
                    {
                        continue;
                    }
                }

                await context.SalCashsaleHds.AddAsync(cashsaleHd);

                var cashSaleDts = request._Cashsale.Where(x => x.JournalId == cashsaleGroup.JournalId);

                int intSeqNo = 1;

                foreach (var cashSaleDt in cashSaleDts)
                {
                    var unitId = string.Empty;
                    var unitName = string.Empty;
                    var unitBarcode = string.Empty;
                    var unitStock = 0;
                    var unitRatio = 0m;

                    var masProduct = context.MasProducts.FirstOrDefault(x => x.PdId == cashSaleDt.PluNumber);

                    if (masProduct != null)
                    {
                        //var masProductUnit = context.MasProductUnits.FirstOrDefault(x => x.PdId == cashSaleDt.PluNumber);
                        var masProductUnit = context.MasProductUnits.FirstOrDefault(x => x.PdId == cashSaleDt.PluNumber && x.UnitId == masProduct.UnitId);

                        if (masProductUnit != null)
                        {
                            var masUnit = context.MasUnits.FirstOrDefault(u => u.UnitId == masProductUnit.UnitId);

                            if (masUnit != null)
                            {
                                unitId = masUnit.UnitId;
                                unitName = masUnit.UnitName;
                            }

                            unitBarcode = masProductUnit.UnitBarcode;
                            unitStock = (int)masProductUnit.UnitStock;
                            unitRatio = (int)masProductUnit.UnitRatio;
                        }
                    }

                    var cashsaleDt = new SalCashsaleDt()
                    {
                        CompCode = request.CompCode,
                        BrnCode = request.BrnCode,
                        LocCode = request.LocCode,
                        DocNo = cashsaleHd.DocNo,
                        SeqNo = intSeqNo++,
                        PdId = cashSaleDt.PluNumber,
                        PdName = masProduct == null ? string.Empty : masProduct.PdName,
                        IsFree = false,
                        UnitId = unitId,
                        UnitBarcode = unitBarcode,
                        UnitName = unitName,
                        ItemQty = cashSaleDt.SelQty,
                        //StockQty = stockQty,
                        StockQty = cashSaleDt.SelQty * unitStock / unitRatio,
                        UnitPrice = cashSaleDt.SalePrice,
                        UnitPriceCur = cashSaleDt.SalePrice,
                        RefPrice = 0m,
                        RefPriceCur = 0m,
                        SumItemAmt = cashSaleDt.SumItemAmount, //cashSaleDt.GoodsAmt + cashSaleDt.TaxAmt + cashSaleDt.DiscAmt,
                        SumItemAmtCur = cashSaleDt.SumItemAmount,//cashSaleDt.GoodsAmt + cashSaleDt.TaxAmt + cashSaleDt.DiscAmt,
                        DiscAmt = cashSaleDt.DiscAmt,
                        DiscAmtCur = cashSaleDt.DiscAmt,
                        DiscHdAmt = 0m,
                        DiscHdAmtCur = 0m,
                        SubAmt = cashSaleDt.SubAmount,//cashSaleDt.GoodsAmt + cashSaleDt.TaxAmt,
                        SubAmtCur = cashSaleDt.SubAmount,//cashSaleDt.GoodsAmt + cashSaleDt.TaxAmt,
                        VatType = "VI",
                        VatRate = 7,
                        VatAmt = cashSaleDt.TaxAmt,
                        VatAmtCur = cashSaleDt.TaxAmt,
                        TaxBaseAmt = cashSaleDt.GoodsAmt,
                        TaxBaseAmtCur = cashSaleDt.GoodsAmt,
                        TotalAmt = cashSaleDt.TotalAmount, //Math.Round((cashSaleDt.GoodsAmt * 107) / 100, 2),
                        TotalAmtCur = cashSaleDt.TotalAmount, //Math.Round((cashSaleDt.GoodsAmt * 107) / 100, 2),
                    };
                    await context.SalCashsaleDts.AddAsync(cashsaleDt);
                }
            }

            //var periodGroup = request._Cashsale.GroupBy(x => x.ShiftNo).Select(group => group.First());
            //var periods = periodGroup.Select(x => x.ShiftNo).ToList();

            var periods = request._Cashsale.Select(x => x.ShiftNo).Distinct();
            var payGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Cash");

            foreach (var period in periods)
            {
                var itemPeriod = request._Cashsale.Where(x => x.ShiftNo == period);
                var periodCount = 0;
                var journalIds = new List<string>();
                var journalIdJson = string.Empty;

                if (itemPeriod != null)
                {
                    periodCount = itemPeriod.Count();
                    journalIds = itemPeriod.Select(x => x.JournalId).ToList();

                    var posLogPeriod = context.DopPos
                    .FirstOrDefault(x => x.CompCode == request.CompCode && x.BrnCode == request.BrnCode && x.LocCode == request.LocCode
                                    && x.DocDate == request.SystemDate
                                    && x.PayGroupId == payGroup.PayGroupId && x.Period == Convert.ToInt32(period));

                    if (posLogPeriod != null)
                    {
                        var oldJournalId = JsonSerializer.Deserialize<List<string>>(posLogPeriod.JsonData);
                        journalIds.AddRange(oldJournalId);
                        journalIdJson = JsonSerializer.Serialize(journalIds);
                        posLogPeriod.JsonData = journalIdJson;
                        posLogPeriod.ItemCount += request._Cashsale.Count();
                        context.DopPos.Update(posLogPeriod);
                    }
                    else
                    {
                        journalIdJson = JsonSerializer.Serialize(journalIds);
                        var pos = new DopPo()
                        {
                            CompCode = request.CompCode,
                            BrnCode = request.BrnCode,
                            LocCode = request.LocCode,
                            DocDate = request.SystemDate,
                            PayGroupId = payGroup.PayGroupId,
                            Period = Convert.ToInt32(period),
                            ItemCount = periodCount,
                            JsonData = journalIdJson,
                            CreatedDate = DateTime.Now
                        };

                        await context.DopPos.AddAsync(pos);
                    }
                }
            }

            var poslog = new DopPosLog()
            {
                CompCode = request.CompCode,
                BrnCode = request.BrnCode,
                LocCode = request.LocCode,
                DocDate = request.SystemDate,
                PayGroupId = payGroup.PayGroupId,
                //Period = Convert.ToInt32(period),
                //ItemCount = periodCount,
                JsonData = JsonSerializer.Serialize(request),
                CreatedDate = DateTime.Now
            };

            await context.DopPosLogs.AddAsync(poslog);
        }

        public async Task AddCreditSaleListAsync(SaveCreditSaleResource request)
        {
            var creditsaleGroups = request._Creditsale
                .Select(x =>
                new
                {
                    x.JournalId,
                    x.SiteId,
                    x.BusinessDate,
                    x.ShiftNo,
                    x.TaxInvNo,
                    x.TotalGoodsAmt,
                    x.TotalDiscAmt,
                    x.TotalTaxAmt,
                    x.TotalPaidAmt,
                    x.PluNumber,
                    x.SelQty,
                    x.SalePrice,
                    x.GoodsAmt,
                    x.TaxAmt,
                    x.DiscAmt,
                    x.BillNo,
                    x.ItemCode,
                    x.ItemName,
                    x.LicensePlate,
                    x.CustCode,
                    x.CustName,
                    x.Po,
                    x.SubAmount,
                    x.TotalAmount,
                    x.EmpCode
                })
                .GroupBy(s => new { s.JournalId, s.SiteId, s.BusinessDate, s.ShiftNo, s.TaxInvNo, s.CustCode, s.EmpCode })
                .Select(g =>
                    new
                    {
                        g.Key.JournalId,
                        g.Key.SiteId,
                        g.Key.BusinessDate,
                        g.Key.ShiftNo,
                        g.Key.TaxInvNo,
                        g.Key.CustCode,
                        g.Key.EmpCode,
                        TotalGoodsAmt = g.Sum(x => x.TotalGoodsAmt),
                        TotalDiscAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.TotalDiscAmt), 2)),
                        TotalTaxAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.TotalTaxAmt), 2)),
                        TotalPaidAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.TotalPaidAmt), 2)),
                        SelQty = g.Sum(x => Math.Round(Convert.ToDecimal(x.SelQty), 2)),
                        SalePrice = g.Sum(x => Math.Round(Convert.ToDecimal(x.SalePrice), 2)),
                        GoodsAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.GoodsAmt), 2)),
                        TaxAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.TaxAmt), 2)),
                        DiscAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.DiscAmt), 2)),
                        request._Creditsale.First().BillNo,
                        request._Creditsale.First().ItemCode,
                        request._Creditsale.First().ItemName,
                        request._Creditsale.First().LicensePlate,
                        request._Creditsale.First().Mile,
                        request._Creditsale.First().CustName,
                        SubAmount = g.Sum(x => Math.Round(Convert.ToDecimal(x.SubAmount), 2)),
                        TotalAmount = g.Sum(x => Math.Round(Convert.ToDecimal(x.TotalAmount), 2)),
                    });

            var docPattern = context.MasDocPatterns.FirstOrDefault(x => x.DocType == "CreditSale");
            var pattern = (docPattern == null) ? string.Empty : docPattern.Pattern;

            pattern = pattern.Replace("Brn", request.BrnCode);
            pattern = pattern.Replace("yy", request.SystemDate.ToString("yy"));
            pattern = pattern.Replace("MM", request.SystemDate.ToString("MM"));

            var runno = GetCreditRunNumber(request.CompCode, request.BrnCode, pattern);
            //var masEmployees = context.MasEmployees.ToList();

            foreach (var creditsaleGroup in creditsaleGroups)
            {
                //var employee = masEmployees.FirstOrDefault(x => x.EmpCode == creditsaleGroup.EmpCode.Trim());
                var creditsaleHd = new SalCreditsaleHd();
                //do
                //{
                //    creditsaleHd.DocNo = GenDocNo(request.SystemDate, request.BrnCode, pattern, ++runno, "CreditSale");
                //} while (await IsDuplicateCreditSale(request,creditsaleHd.DocNo));
                //creditsaleHd.RunNumber = runno;
                creditsaleHd.RunNumber = ++runno;
                creditsaleHd.CompCode = request.CompCode;
                creditsaleHd.BrnCode = request.BrnCode;
                creditsaleHd.LocCode = request.LocCode;
                creditsaleHd.DocType = "CreditSale";
                creditsaleHd.DocNo = GenDocNo(request.SystemDate, request.BrnCode, pattern, (int)creditsaleHd.RunNumber, "CreditSale");
                creditsaleHd.DocStatus = "Active";
                creditsaleHd.DocDate = request.SystemDate;
                creditsaleHd.Period = null;
                creditsaleHd.RefNo = creditsaleGroup.JournalId;
                creditsaleHd.CustCode = creditsaleGroup.CustCode;

                var customer = context.MasCustomers.FirstOrDefault(x => x.CustCode == creditsaleHd.CustCode);
                if (customer != null)
                {
                    creditsaleHd.CustName = customer.CustPrefix + " " + customer.CustName;
                    creditsaleHd.CustAddr1 = customer.CustAddr1;
                    creditsaleHd.CustAddr2 = customer.CustAddr2;
                    creditsaleHd.CitizenId = customer.CitizenId ?? "";
                }

                creditsaleHd.ItemCount = 0;
                creditsaleHd.Remark = null;
                creditsaleHd.Currency = "THB";
                creditsaleHd.CurRate = 1m;
                creditsaleHd.SubAmt = creditsaleGroup.SubAmount; //creditsaleGroup.GoodsAmt + creditsaleGroup.TaxAmt;
                creditsaleHd.SubAmtCur = creditsaleGroup.SubAmount; //creditsaleGroup.GoodsAmt + creditsaleGroup.TaxAmt;
                creditsaleHd.DiscRate = string.Empty;
                creditsaleHd.DiscAmt = 0m;
                creditsaleHd.DiscAmtCur = 0m;
                //creditsaleHd.TotalAmt = creditsaleHd.SubAmt - creditsaleHd.DiscAmt; //creditsaleGroup.TotalAmount;//creditsaleGroup.GoodsAmt + creditsaleGroup.TaxAmt;
                //creditsaleHd.TotalAmtCur = creditsaleHd.SubAmtCur - creditsaleHd.DiscAmtCur;//creditsaleGroup.GoodsAmt + creditsaleGroup.TaxAmt;
                creditsaleHd.TotalAmt = creditsaleGroup.TaxAmt + creditsaleGroup.GoodsAmt; // VatAmt + TaxBaseAmt
                creditsaleHd.TotalAmtCur = creditsaleGroup.TaxAmt + creditsaleGroup.GoodsAmt; // VatAmt + TaxBaseAmt
                creditsaleHd.TaxBaseAmt = creditsaleGroup.GoodsAmt;
                creditsaleHd.TaxBaseAmtCur = creditsaleGroup.GoodsAmt;
                creditsaleHd.VatRate = 0;
                creditsaleHd.VatAmt = creditsaleGroup.TaxAmt;
                creditsaleHd.VatAmtCur = creditsaleGroup.TaxAmt;
                creditsaleHd.NetAmt = creditsaleGroup.GoodsAmt + creditsaleGroup.TaxAmt;
                creditsaleHd.NetAmtCur = creditsaleGroup.GoodsAmt + creditsaleGroup.TaxAmt;
                creditsaleHd.TxNo = null;
                creditsaleHd.Post = "N";
                creditsaleHd.DocPattern = pattern;
                creditsaleHd.CreatedDate = DateTime.Now;
                creditsaleHd.CreatedBy = request.CreatedBy;
                creditsaleHd.UpdatedDate = DateTime.Now;
                //creditsaleHd.EmpCode = employee != null ? employee.EmpCode : string.Empty;
                //creditsaleHd.EmpName = employee != null ? employee.EmpName : string.Empty;

                await context.SalCreditsaleHds.AddAsync(creditsaleHd);

                var creditSaleDts = request._Creditsale.Where(x => x.JournalId == creditsaleGroup.JournalId);
                int intSeqNo = 1;

                foreach (var creditSaleDt in creditSaleDts)
                {
                    var unitId = string.Empty;
                    var unitName = string.Empty;
                    var unitBarcode = string.Empty;
                    var unitStock = 0;
                    var unitRatio = 0m;

                    var masProduct = context.MasProducts.FirstOrDefault(x => x.PdId == creditSaleDt.PluNumber);

                    if (masProduct != null)
                    {
                        //var masProductUnit = context.MasProductUnits.FirstOrDefault(x => x.PdId == creditSaleDt.PluNumber);
                        var masProductUnit = context.MasProductUnits.FirstOrDefault(x => x.PdId == creditSaleDt.PluNumber && x.UnitId == masProduct.UnitId);

                        if (masProductUnit != null)
                        {
                            var masUnit = context.MasUnits.FirstOrDefault(u => u.UnitId == masProductUnit.UnitId);

                            if (masUnit != null)
                            {
                                unitId = masUnit.UnitId;
                                unitName = masUnit.UnitName;
                            }
                            unitBarcode = masProductUnit.UnitBarcode;
                            unitStock = (int)masProductUnit.UnitStock;
                            unitRatio = (int)masProductUnit.UnitRatio;
                        }

                    }

                    var creditsaleDt = new SalCreditsaleDt();
                    creditsaleDt.CompCode = request.CompCode;
                    creditsaleDt.BrnCode = request.BrnCode;
                    creditsaleDt.LocCode = request.LocCode;
                    creditsaleDt.DocType = "CreditSale";
                    creditsaleDt.DocNo = creditsaleHd.DocNo;
                    creditsaleDt.SeqNo = intSeqNo++;
                    creditsaleDt.PoNo = creditSaleDt.Po;
                    creditsaleDt.LicensePlate = creditSaleDt.LicensePlate;

                    if (int.TryParse(creditSaleDt.Mile, out int temp))
                    {
                        creditsaleDt.Mile = Convert.ToInt32(creditSaleDt.Mile);
                    }
                    else
                    {
                        creditsaleDt.Mile = 0;
                    }

                    creditsaleDt.PdId = creditSaleDt.PluNumber;
                    creditsaleDt.PdName = masProduct == null ? string.Empty : masProduct.PdName;
                    creditsaleDt.IsFree = false;
                    creditsaleDt.UnitId = unitId;
                    creditsaleDt.UnitBarcode = unitBarcode;
                    creditsaleDt.UnitName = unitName;
                    creditsaleDt.ItemQty = creditSaleDt?.SelQty ?? decimal.Zero;
                    //creditsaleDt.StockQty = creditSaleDt?.SelQty??decimal.Zero;
                    creditsaleDt.StockQty = creditSaleDt.SelQty * unitStock / unitRatio;
                    creditsaleDt.UnitPrice = creditSaleDt.SalePrice;
                    creditsaleDt.UnitPriceCur = creditSaleDt.SalePrice;
                    creditsaleDt.RefPrice = 0m;
                    creditsaleDt.RefPriceCur = 0m;
                    creditsaleDt.SumItemAmt = creditSaleDt.SumItemAmount;//creditSaleDt.GoodsAmt + creditSaleDt.TaxAmt + creditSaleDt.DiscAmt;
                    creditsaleDt.SumItemAmtCur = creditSaleDt.SumItemAmount;//creditSaleDt.GoodsAmt + creditSaleDt.TaxAmt + creditSaleDt.DiscAmt;
                    creditsaleDt.DiscAmt = creditSaleDt.DiscAmt;
                    creditsaleDt.DiscAmtCur = creditSaleDt.DiscAmt;
                    creditsaleDt.DiscHdAmt = 0m;
                    creditsaleDt.DiscHdAmtCur = 0m;
                    creditsaleDt.SubAmt = creditSaleDt.SubAmount;//creditSaleDt.GoodsAmt + creditSaleDt.TaxAmt;
                    creditsaleDt.SubAmtCur = creditSaleDt.SubAmount;//creditSaleDt.GoodsAmt + creditSaleDt.TaxAmt;
                    creditsaleDt.VatType = "VI";
                    creditsaleDt.VatRate = 7;
                    creditsaleDt.VatAmt = creditSaleDt.TaxAmt;
                    creditsaleDt.VatAmtCur = creditSaleDt.TaxAmt;
                    creditsaleDt.TaxBaseAmt = creditSaleDt.GoodsAmt;
                    creditsaleDt.TaxBaseAmtCur = creditSaleDt.GoodsAmt;
                    creditsaleDt.TotalAmt = creditsaleDt.VatAmt + creditsaleDt.TaxBaseAmt; //creditSaleDt.TotalAmount; //Math.Round((creditSaleDt.GoodsAmt * 107) / 100, 2);
                    creditsaleDt.TotalAmtCur = creditsaleDt.VatAmtCur + creditsaleDt.TaxBaseAmtCur;  //creditSaleDt.TotalAmount; //Math.Round((creditSaleDt.GoodsAmt * 107) / 100, 2);


                    await context.SalCreditsaleDts.AddAsync(creditsaleDt);
                }
            }

            //var periodGroup = request._Creditsale.GroupBy(x => x.ShiftNo).Select(group => group.First());
            //var periods = periodGroup.Select(x => x.ShiftNo).ToList();
            var periods = request._Creditsale.Select(x => x.ShiftNo).Distinct();
            var payGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Credit");

            foreach (var period in periods)
            {
                var itemPeriod = request._Creditsale.Where(x => x.ShiftNo == period);
                var periodCount = 0;
                var journalIds = new List<string>();
                var journalIdJson = string.Empty;

                if (itemPeriod != null)
                {
                    periodCount = itemPeriod.Count();
                    journalIds = itemPeriod.Select(x => x.JournalId).ToList();

                    var posLogPeriod = context.DopPos
                    .FirstOrDefault(x => x.CompCode == request.CompCode && x.BrnCode == request.BrnCode && x.LocCode == request.LocCode
                                    && x.DocDate == request.SystemDate
                                    && x.PayGroupId == payGroup.PayGroupId && x.Period == Convert.ToInt32(period));

                    if (posLogPeriod != null)
                    {
                        var oldJournalId = JsonSerializer.Deserialize<List<string>>(posLogPeriod.JsonData);
                        journalIds.AddRange(oldJournalId);
                        journalIdJson = JsonSerializer.Serialize(journalIds);
                        posLogPeriod.JsonData = journalIdJson;
                        posLogPeriod.ItemCount += request._Creditsale.Count();
                        context.DopPos.Update(posLogPeriod);
                    }
                    else
                    {
                        journalIdJson = JsonSerializer.Serialize(journalIds);
                        var pos = new DopPo()
                        {
                            CompCode = request.CompCode,
                            BrnCode = request.BrnCode,
                            LocCode = request.LocCode,
                            DocDate = request.SystemDate,
                            PayGroupId = payGroup.PayGroupId,
                            Period = Convert.ToInt32(period),
                            ItemCount = periodCount,
                            JsonData = journalIdJson,
                            CreatedDate = DateTime.Now
                        };

                        await context.DopPos.AddAsync(pos);
                    }
                }
            }

            var poslog = new DopPosLog()
            {
                CompCode = request.CompCode,
                BrnCode = request.BrnCode,
                LocCode = request.LocCode,
                DocDate = request.SystemDate,
                PayGroupId = payGroup.PayGroupId,
                //Period = Convert.ToInt32(period),
                //ItemCount = periodCount,
                JsonData = JsonSerializer.Serialize(request),
                CreatedDate = DateTime.Now
            };

            await context.DopPosLogs.AddAsync(poslog);
        }

        public async Task AddWithdrawListAsync(SaveWithdrawResource request)
        {
            //var withdrawGroups = request._Withdraw
            //    .Select(x =>
            //    new
            //    {
            //        x.SiteId,
            //        x.BusinessDate,
            //        x.ShiftNo,
            //        x.UserBrnCode,
            //        x.LicensePlate,
            //        x.EmpCode,
            //        x.EmpName,
            //        x.ReasonId,
            //        x.PluNumber
            //        //x.JournalId,
            //    })
            //    //.GroupBy(s => new { s.JournalId, s.SiteId, s.BusinessDate, s.ShiftNo, s.TaxInvNo, s.UserBrnCode, s.LicensePlate, s.EmpCode, s.EmpName, s.ReasonId })
            //    .GroupBy(s => new { s.SiteId, s.BusinessDate, s.ShiftNo, s.UserBrnCode, s.LicensePlate, s.EmpCode, s.EmpName, s.ReasonId, s.PluNumber })
            //    .Select(g =>
            //        new
            //        {
            //            g.Key.SiteId,
            //            g.Key.BusinessDate,
            //            g.Key.ShiftNo,
            //            g.Key.UserBrnCode,
            //            g.Key.LicensePlate,
            //            g.Key.EmpCode,
            //            g.Key.EmpName,
            //            g.Key.ReasonId,
            //            g.Key.PluNumber
            //        });

            var docPattern = context.MasDocPatterns.FirstOrDefault(x => x.DocType == "Withdraw");
            var pattern = (docPattern == null) ? string.Empty : docPattern.Pattern;

            pattern = pattern.Replace("Brn", request.BrnCode);
            pattern = pattern.Replace("yy", request.SystemDate.ToString("yy"));
            pattern = pattern.Replace("MM", request.SystemDate.ToString("MM"));

            var runno = GetWithdrawRunNumber(request.CompCode, request.BrnCode, pattern);

            foreach (var withdrawGroup in request._Withdraw)
            {
                var withdrawHd = new InvWithdrawHd();
                //do
                //{
                //    withdrawHd.DocNo = GenDocNo(request.SystemDate, request.BrnCode, pattern, ++runno, "Withdraw");
                //} while (await IsDuplicateWithDraw(request , withdrawHd.DocNo));
                //withdrawHd.RunNumber = runno;
                withdrawHd.RunNumber = ++runno;
                withdrawHd.CompCode = request.CompCode;
                withdrawHd.BrnCode = request.BrnCode;
                withdrawHd.LocCode = request.LocCode;
                withdrawHd.DocNo = GenDocNo(request.SystemDate, request.BrnCode, pattern, (int)withdrawHd.RunNumber, "Withdraw");
                withdrawHd.DocStatus = "Active";
                withdrawHd.DocDate = request.SystemDate;
                withdrawHd.UseBrnCode = withdrawGroup.UserBrnCode;
                withdrawHd.CreatedBy = request.CreateBy;

                var costcenter = context.MasCostCenters.FirstOrDefault(x => x.BrnCode == withdrawGroup.UserBrnCode && x.CompCode == withdrawGroup.CustCode);

                if (costcenter != null)
                {
                    withdrawHd.UseBrnName = costcenter.BrnName;
                }

                withdrawHd.LicensePlate = withdrawGroup.LicensePlate;
                withdrawHd.EmpCode = withdrawGroup.EmpCode;

                var employee = context.MasEmployees.FirstOrDefault(x => x.EmpCode == withdrawHd.EmpCode);

                if (employee != null)
                {
                    withdrawHd.EmpName = employee.EmpName;
                }

                withdrawHd.ReasonId = withdrawGroup.ReasonId;

                var reason = context.MasReasons.FirstOrDefault(x => x.ReasonId == withdrawHd.ReasonId && x.ReasonGroup == "Withdraw");

                if (reason != null)
                {
                    withdrawHd.ReasonDesc = reason.ReasonDesc;
                }

                withdrawHd.Remark = withdrawGroup.Remark;
                withdrawHd.RefNo = "POS";
                withdrawHd.Post = "N";
                withdrawHd.DocPattern = pattern;
                withdrawHd.CreatedDate = DateTime.Now;
                withdrawHd.CreatedBy = request.CreateBy;

                await context.InvWithdrawHds.AddAsync(withdrawHd);

                int intSeqNo = 1;

                var masProduct = context.MasProducts.FirstOrDefault(x => x.PdId == withdrawGroup.PluNumber);
                var masProductUnit = context.MasProductUnits.FirstOrDefault(x => x.PdId == withdrawGroup.PluNumber);
                var masUnit = context.MasUnits.FirstOrDefault(u => u.UnitId == masProductUnit.UnitId);

                var invWithdrawDt = new InvWithdrawDt()
                {
                    CompCode = request.CompCode,
                    BrnCode = request.BrnCode,
                    LocCode = request.LocCode,
                    DocNo = withdrawHd.DocNo,
                    SeqNo = intSeqNo++,
                    PdId = withdrawGroup.PluNumber,
                    PdName = masProduct == null ? string.Empty : masProduct.PdName,
                    UnitId = masUnit.UnitId,
                    UnitBarcode = masProductUnit.UnitBarcode,
                    UnitName = masUnit.UnitName,
                    StockQty = withdrawGroup.SelQty,
                    ItemQty = withdrawGroup.SelQty
                };

                await context.InvWithdrawDts.AddAsync(invWithdrawDt);

                //var withdrawDts = request._Withdraw.OrderBy(x => x.PluNumber).Where(x => x.JournalId == withdrawGroup.PluNumber);

                //foreach (var withdrawDt in request._Withdraw)
                //{
                //    if (withdrawHd != null)
                //    {
                //        var masProduct = context.MasProducts.FirstOrDefault(x => x.PdId == withdrawDt.PluNumber);
                //        var masProductUnit = context.MasProductUnits.FirstOrDefault(x => x.PdId == withdrawDt.PluNumber);
                //        var masUnit = context.MasUnits.FirstOrDefault(u => u.UnitId == masProductUnit.UnitId);

                //        var invWithdrawDt = new InvWithdrawDt()
                //        {
                //            CompCode = request.CompCode,
                //            BrnCode = request.BrnCode,
                //            LocCode = request.LocCode,
                //            DocNo = withdrawHd.DocNo,
                //            SeqNo = intSeqNo++,
                //            PdId = withdrawDt.PluNumber,
                //            PdName = masProduct == null ? string.Empty : masProduct.PdName,
                //            UnitId = masUnit.UnitId,
                //            UnitBarcode = masProductUnit.UnitBarcode,
                //            UnitName = masUnit.UnitName,
                //            StockQty = withdrawDt.SelQty,
                //            ItemQty = withdrawDt.SelQty
                //        };

                //        await context.InvWithdrawDts.AddAsync(invWithdrawDt);
                //    }
                //}
            }

            var periods = request._Withdraw.Select(x => x.ShiftNo).Distinct();
            var payGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Withdraw");

            foreach (var period in periods)
            {
                var itemPeriod = request._Withdraw.Where(x => x.ShiftNo == period);
                var periodCount = 0;
                var journalIds = new List<string>();
                var journalIdJson = string.Empty;

                if (itemPeriod != null)
                {
                    periodCount = itemPeriod.Count();
                    journalIds = itemPeriod.Select(x => x.JournalId).ToList();

                    var posLogPeriod = context.DopPos
                    .FirstOrDefault(x => x.CompCode == request.CompCode && x.BrnCode == request.BrnCode && x.LocCode == request.LocCode
                                    && x.CreatedDate == request.SystemDate
                                    && x.PayGroupId == payGroup.PayGroupId && x.Period == Convert.ToInt32(period));

                    if (posLogPeriod != null)
                    {
                        var oldJournalId = JsonSerializer.Deserialize<List<string>>(posLogPeriod.JsonData);
                        journalIds.AddRange(oldJournalId);
                        journalIdJson = JsonSerializer.Serialize(journalIds);
                        posLogPeriod.JsonData = journalIdJson;
                        posLogPeriod.ItemCount += request._Withdraw.Count();
                        context.DopPos.Update(posLogPeriod);
                    }
                    else
                    {
                        journalIdJson = JsonSerializer.Serialize(journalIds);
                        var pos = new DopPo()
                        {
                            CompCode = request.CompCode,
                            BrnCode = request.BrnCode,
                            LocCode = request.LocCode,
                            DocDate = request.SystemDate,
                            PayGroupId = payGroup.PayGroupId,
                            Period = Convert.ToInt32(period),
                            ItemCount = periodCount,
                            JsonData = journalIdJson,
                            CreatedDate = DateTime.Now
                        };

                        await context.DopPos.AddAsync(pos);
                    }
                }
            }

            var poslog = new DopPosLog()
            {
                CompCode = request.CompCode,
                BrnCode = request.BrnCode,
                LocCode = request.LocCode,
                DocDate = request.SystemDate,
                PayGroupId = payGroup.PayGroupId,
                //Period = Convert.ToInt32(period),
                //ItemCount = periodCount,
                JsonData = JsonSerializer.Serialize(request),
                CreatedDate = DateTime.Now
            };

            await context.DopPosLogs.AddAsync(poslog);
        }

        public async Task AddReceiveListAsync(SaveReceiveResource request)
        {
            var receiveGroups = request._POSReceives
                .Select(x =>
                new
                {
                    x.JournalId,
                    x.CustCode,
                    x.SiteId,
                    x.BusinessDate,
                    x.ShiftNo,
                    x.PluNumber,
                    x.ItemName,
                    x.SellQty,
                    x.SellPrice,
                    x.GoodsAmt,
                    x.TaxAmt,
                    x.DiscAmt,
                    x.SumItemAmt,
                    x.SubAmt,
                    x.TotalAmt,
                })
                .GroupBy(s => new { s.JournalId, s.SiteId, s.BusinessDate, s.ShiftNo, s.CustCode })
                .Select(g =>
                    new
                    {
                        g.Key.JournalId,
                        g.Key.CustCode,
                        g.Key.SiteId,
                        g.Key.BusinessDate,
                        g.Key.ShiftNo,
                        request._POSReceives.First().PluNumber,
                        request._POSReceives.First().ItemName,
                        SellQty = g.Sum(x => Math.Round(Convert.ToDecimal(x.SellQty), 2)),
                        SellPrice = g.Sum(x => Math.Round(Convert.ToDecimal(x.SellPrice), 2)),
                        GoodsAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.GoodsAmt), 2)),
                        TaxAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.TaxAmt), 2)),
                        DiscAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.DiscAmt), 2)),
                        SumItemAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.SumItemAmt), 2)),
                        SubAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.SubAmt), 2)),
                        TotalAmt = g.Sum(x => Math.Round(Convert.ToDecimal(x.TotalAmt), 2)),
                    });

            var docPattern = context.MasDocPatterns.FirstOrDefault(x => x.DocType == "Receive");
            var pattern = (docPattern == null) ? string.Empty : docPattern.Pattern;

            pattern = pattern.Replace("Brn", request.BrnCode);
            pattern = pattern.Replace("yy", request.SystemDate.ToString("yy"));
            pattern = pattern.Replace("MM", request.SystemDate.ToString("MM"));

            var runno = GetReceiveRunNumber(request.CompCode, request.BrnCode, pattern);
            var masProducts = context.MasProducts;
            var customers = context.MasCustomers;
            var productDisCount = context.DopPosConfigs.FirstOrDefault(x => x.DocType == "Receive" && x.DocDesc == "Dicsount");

            foreach (var receiveGroup in receiveGroups)
            {
                var customer = customers.FirstOrDefault(x => x.CustCode == receiveGroup.CustCode);
                var receiveHd = new FinReceiveHd();
                receiveHd.RunNumber = ++runno;
                receiveHd.CompCode = request.CompCode;
                receiveHd.BrnCode = request.BrnCode;
                receiveHd.LocCode = request.LocCode;
                receiveHd.DocNo = GenDocNo(request.SystemDate, request.BrnCode, pattern, (int)receiveHd.RunNumber, "Receive");
                receiveHd.DocStatus = "Active";
                receiveHd.DocDate = request.SystemDate;
                receiveHd.ReceiveTypeId = "3";
                receiveHd.ReceiveType = "รับอื่นๆ";
                receiveHd.BillNo = receiveGroup.JournalId;
                receiveHd.CustCode = receiveGroup.CustCode;

                if (customer != null)
                {
                    receiveHd.CustName = customer.CustPrefix + " " + customer.CustName;
                    receiveHd.CustAddr1 = customer.CustAddr1;
                    receiveHd.CustAddr2 = customer.CustAddr2;
                }

                receiveHd.PayTypeId = "1";
                receiveHd.PayType = "เงินสด";
                receiveHd.PayDate = null;
                receiveHd.BankNo = string.Empty;
                receiveHd.BankName = string.Empty;
                receiveHd.AccountNo = string.Empty;
                receiveHd.PayNo = string.Empty;
                receiveHd.Remark = null;
                receiveHd.SubAmt = receiveGroup.SubAmt;
                receiveHd.SubAmtCur = receiveGroup.SubAmt;
                receiveHd.FeeAmt = 0m;
                receiveHd.FeeAmtCur = 0m;
                receiveHd.WhtAmt = 0m;
                receiveHd.WhtAmtCur = 0m;
                receiveHd.DiscAmt = receiveGroup.DiscAmt;
                receiveHd.DiscAmtCur = receiveGroup.DiscAmt;
                receiveHd.TotalAmt = receiveGroup.TotalAmt;
                receiveHd.TotalAmtCur = receiveGroup.TotalAmt;
                receiveHd.VatType = string.Empty;
                receiveHd.VatRate = 7;
                receiveHd.NetAmt = receiveGroup.GoodsAmt + receiveGroup.TaxAmt;
                receiveHd.NetAmtCur = receiveGroup.GoodsAmt + receiveGroup.TaxAmt;
                receiveHd.Post = "N";
                receiveHd.DocPattern = pattern;
                receiveHd.CreatedDate = DateTime.Now;
                receiveHd.CreatedBy = request.CreatedBy;
                receiveHd.UpdatedDate = DateTime.Now;

                var receiveDts = request._POSReceives.Where(x => x.JournalId == receiveGroup.JournalId);
                int intSeqNo = 1;
                decimal sumTaxAmt = 0m;
                decimal discount = 0m;

                foreach (var receiveDt in receiveDts)
                {
                    if (receiveGroup.DiscAmt == 0)
                    {
                        var masProduct = masProducts.FirstOrDefault(x => x.PdId == receiveDt.PluNumber);
                        var finReceiveDt = new FinReceiveDt();
                        finReceiveDt.CompCode = request.CompCode;
                        finReceiveDt.BrnCode = request.BrnCode;
                        finReceiveDt.LocCode = request.LocCode;
                        finReceiveDt.SeqNo = intSeqNo++;
                        finReceiveDt.DocNo = receiveHd.DocNo;
                        finReceiveDt.AccountNo = masProduct.AcctCode;
                        finReceiveDt.Remark = string.Empty;
                        finReceiveDt.ItemAmt = receiveDt.SumItemAmt;
                        finReceiveDt.ItemAmtCur = receiveDt.SumItemAmt;
                        finReceiveDt.PdId = masProduct.PdId;
                        finReceiveDt.PdName = masProduct == null ? string.Empty : masProduct.PdName;
                        discount = 0;

                        if (masProduct != null)
                        {
                            finReceiveDt.VatType = masProduct.VatType;
                            finReceiveDt.VatRate = masProduct.VatRate;
                            var taxAmt = 0m;

                            if (masProduct.VatType == "VE")
                            {
                                taxAmt = (decimal)((receiveDt.SubAmt - receiveDt.DiscAmt) * masProduct.VatRate / 100);
                                finReceiveDt.VatAmt = taxAmt;
                                finReceiveDt.VatAmtCur = taxAmt;
                                sumTaxAmt += taxAmt;
                            }
                            else if (masProduct.VatType == "VI")
                            {
                                taxAmt = (decimal)((receiveDt.SubAmt - receiveDt.DiscAmt) * masProduct.VatRate / (100 + masProduct.VatRate));
                                finReceiveDt.VatAmt = taxAmt;
                                finReceiveDt.VatAmtCur = taxAmt;
                                sumTaxAmt += taxAmt;
                            }
                            else if (masProduct.VatType == "NV")
                            {
                                finReceiveDt.VatAmt = 0m;
                                finReceiveDt.VatAmtCur = 0m;
                            }
                        }
                        await context.FinReceiveDts.AddAsync(finReceiveDt);
                    }
                    else if (receiveGroup.DiscAmt != 0)
                    {
                        var masProduct = masProducts.FirstOrDefault(x => x.PdId == receiveDt.PluNumber);
                        var finReceiveDt = new FinReceiveDt();
                        finReceiveDt.CompCode = request.CompCode;
                        finReceiveDt.BrnCode = request.BrnCode;
                        finReceiveDt.LocCode = request.LocCode;
                        finReceiveDt.SeqNo = intSeqNo++;
                        finReceiveDt.DocNo = receiveHd.DocNo;
                        finReceiveDt.AccountNo = masProduct.AcctCode;
                        finReceiveDt.Remark = string.Empty;
                        finReceiveDt.ItemAmt = receiveDt.SumItemAmt;
                        finReceiveDt.ItemAmtCur = receiveDt.SumItemAmt;
                        finReceiveDt.PdId = masProduct.PdId;
                        finReceiveDt.PdName = masProduct == null ? string.Empty : masProduct.PdName;

                        if (masProduct != null)
                        {
                            finReceiveDt.VatType = masProduct.VatType;
                            finReceiveDt.VatRate = masProduct.VatRate;
                            var taxAmt = 0m;

                            if (masProduct.VatType == "VE")
                            {
                                taxAmt = (decimal)((receiveDt.SubAmt - receiveDt.DiscAmt) * masProduct.VatRate / 100);
                                finReceiveDt.VatAmt = taxAmt;
                                finReceiveDt.VatAmtCur = taxAmt;
                                sumTaxAmt += taxAmt;
                            }
                            else if (masProduct.VatType == "VI")
                            {
                                taxAmt = (decimal)((receiveDt.SubAmt - receiveDt.DiscAmt) * masProduct.VatRate / (100 + masProduct.VatRate));
                                finReceiveDt.VatAmt = taxAmt;
                                finReceiveDt.VatAmtCur = taxAmt;
                                sumTaxAmt += taxAmt;
                            }
                            else if (masProduct.VatType == "NV")
                            {
                                finReceiveDt.VatAmt = 0m;
                                finReceiveDt.VatAmtCur = 0m;
                            }
                        }
                        await context.FinReceiveDts.AddAsync(finReceiveDt);

                        var productDiscount = masProducts.FirstOrDefault(x => x.PdId == productDisCount.PdId);
                        var finReceiveDtDiscount = new FinReceiveDt();
                        finReceiveDtDiscount.CompCode = request.CompCode;
                        finReceiveDtDiscount.BrnCode = request.BrnCode;
                        finReceiveDtDiscount.LocCode = request.LocCode;
                        finReceiveDtDiscount.SeqNo = intSeqNo++;
                        finReceiveDtDiscount.DocNo = receiveHd.DocNo;
                        finReceiveDtDiscount.AccountNo = productDiscount.AcctCode;
                        finReceiveDtDiscount.Remark = string.Empty;
                        finReceiveDtDiscount.ItemAmt = Math.Abs(receiveGroup.DiscAmt) * (-1);
                        finReceiveDtDiscount.ItemAmtCur = Math.Abs(receiveGroup.DiscAmt) * (-1);
                        finReceiveDtDiscount.PdId = productDiscount == null ? string.Empty : productDiscount.PdId;
                        finReceiveDtDiscount.PdName = productDiscount == null ? string.Empty : productDiscount.PdName;
                        discount = receiveGroup.DiscAmt - receiveGroup.DiscAmt;

                        if (productDiscount != null)
                        {
                            finReceiveDtDiscount.VatType = productDiscount.VatType;
                            finReceiveDtDiscount.VatRate = productDiscount.VatRate;
                            var taxAmt = 0m;

                            if (productDiscount.VatType == "VE")
                            {
                                taxAmt = (decimal)((receiveDt.SubAmt - receiveDt.DiscAmt) * productDiscount.VatRate / 100);
                                finReceiveDtDiscount.VatAmt = taxAmt;
                                finReceiveDtDiscount.VatAmtCur = taxAmt;
                                sumTaxAmt += taxAmt;
                            }
                            else if (productDiscount.VatType == "VI")
                            {
                                taxAmt = (decimal)((receiveDt.SubAmt - receiveDt.DiscAmt) * productDiscount.VatRate / (100 + productDiscount.VatRate));
                                finReceiveDtDiscount.VatAmt = taxAmt;
                                finReceiveDtDiscount.VatAmtCur = taxAmt;
                                sumTaxAmt += taxAmt;
                            }
                            else if (productDiscount.VatType == "NV")
                            {
                                finReceiveDtDiscount.VatAmt = 0m;
                                finReceiveDtDiscount.VatAmtCur = 0m;
                            }
                        }

                        await context.FinReceiveDts.AddAsync(finReceiveDtDiscount);
                    }
                }

                receiveHd.VatAmt = sumTaxAmt;
                receiveHd.VatAmtCur = sumTaxAmt;
                receiveHd.DiscAmt = discount;
                receiveHd.DiscAmtCur = discount;

                await context.FinReceiveHds.AddAsync(receiveHd);
            }

            var periods = request._POSReceives.Select(x => x.ShiftNo).Distinct();
            var payGroup = context.MasPayGroups.FirstOrDefault(x => x.PayGroupName == "Receive");

            foreach (var period in periods)
            {
                var itemPeriod = request._POSReceives.Where(x => x.ShiftNo == period);
                var periodCount = 0;
                var journalIds = new List<string>();
                var journalIdJson = string.Empty;

                if (itemPeriod != null)
                {
                    periodCount = itemPeriod.Count();
                    journalIds = itemPeriod.Select(x => x.JournalId).ToList();

                    var posLogPeriod = context.DopPos
                    .FirstOrDefault(x => x.CompCode == request.CompCode && x.BrnCode == request.BrnCode && x.LocCode == request.LocCode
                                    && x.DocDate == request.SystemDate
                                    && x.PayGroupId == payGroup.PayGroupId && x.Period == Convert.ToInt32(period));

                    if (posLogPeriod != null)
                    {
                        var oldJournalId = JsonSerializer.Deserialize<List<string>>(posLogPeriod.JsonData);
                        journalIds.AddRange(oldJournalId);
                        journalIdJson = JsonSerializer.Serialize(journalIds);
                        posLogPeriod.JsonData = journalIdJson;
                        posLogPeriod.ItemCount += request._POSReceives.Count();
                        context.DopPos.Update(posLogPeriod);
                    }
                    else
                    {
                        journalIdJson = JsonSerializer.Serialize(journalIds);
                        var pos = new DopPo()
                        {
                            CompCode = request.CompCode,
                            BrnCode = request.BrnCode,
                            LocCode = request.LocCode,
                            DocDate = request.SystemDate,
                            PayGroupId = payGroup.PayGroupId,
                            Period = Convert.ToInt32(period),
                            ItemCount = periodCount,
                            JsonData = journalIdJson,
                            CreatedDate = DateTime.Now
                        };

                        await context.DopPos.AddAsync(pos);
                    }
                }
            }

            var poslog = new DopPosLog()
            {
                CompCode = request.CompCode,
                BrnCode = request.BrnCode,
                LocCode = request.LocCode,
                DocDate = request.SystemDate,
                PayGroupId = payGroup.PayGroupId,
                JsonData = JsonSerializer.Serialize(request),
                CreatedDate = DateTime.Now
            };

            await context.DopPosLogs.AddAsync(poslog);
        }

        public DopPosConfig GetWithdrawStatus(WithdrawStatusRequest req)
        {
            var masDopPosConfig = context.DopPosConfigs.FirstOrDefault(
                x => (x.DocType == req.DocType && x.DocDesc == "Checkbox")
            );
            return masDopPosConfig;
        }

        public async Task<DopPosConfig[]> GetDopPosConfig(GetDopPosConfigParam param)
        {
            DopPosConfig[] result = null;
            if (param != null)
            {
                var qryDopPosConfig = context.DopPosConfigs.AsQueryable();
                if (param.ArrDocType != null && param.ArrDocType.Length > 0)
                {
                    qryDopPosConfig = qryDopPosConfig.Where(x => param.ArrDocType.Contains(x.DocType));
                }
                if (param.ArrDocDesc != null && param.ArrDocDesc.Length > 0)
                {
                    qryDopPosConfig = qryDopPosConfig.Where(x => param.ArrDocDesc.Contains(x.DocDesc));
                }
                result = await Task.Run(() => qryDopPosConfig.ToArray());
            }
            return result;
        }

        public List<string> ValidateCustomer(SaveCreditSaleResource req)
        {
            var customerIsValid = new List<string>();
            foreach (var csaveCreditSale in req._Creditsale)
            {
                var customer = context.MasCustomers.FirstOrDefault(x => x.CustCode == csaveCreditSale.CustCode);

                if (customer == null)
                {
                    customerIsValid.Add(csaveCreditSale.CustCode);
                }
            }
            return customerIsValid;
        }

        public List<string> ValidateMasProduct(IEnumerable<string> productIds)
        {
            var masProductIsValid = new List<string>();

            foreach (var productId in productIds)
            {
                var masProduct = context.MasProducts.FirstOrDefault(x => x.PdId == productId);

                if (masProduct == null)
                {
                    masProductIsValid.Add(productId);
                }
            }
            return masProductIsValid;
        }

        public List<string> ValidateMasProductUnit(IEnumerable<string> productIds)
        {
            var masProductIsValid = new List<string>();

            foreach (var productId in productIds)
            {
                var masProduct = context.MasProductUnits.FirstOrDefault(x => x.PdId == productId);

                if (masProduct == null)
                {
                    masProductIsValid.Add(productId);
                }
            }
            return masProductIsValid;
        }

        public CreditSummaryResponse GetCreditSummaryByBranch(CreditSummaryRequest request)
        {
            var response = new CreditSummaryResponse();
            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 60,
                CommandText = $@"SELECT DISTINCT SHIFT_NO FROM {schema}FUNCTION2 WHERE SITE_ID = '52{request.BrnCode}' AND TRUNC(FUNCTION2.BUSINESS_DATE) = TO_DATE('{request.FromDate:dd/MM/yyyy}','dd/MM/yyyy') "

            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();

            var shiftNoList = new List<string>();

            while (oracleDataReader.Read())
            {
                var shiftNo = oracleDataReader["SHIFT_NO"].ToString();
                shiftNoList.Add(shiftNo);
            }

            oracleDataReader.Close();
            oracleDataReader.Dispose();
            con.Close();
            var mopCode = new List<int>() { 2, 3, 11, 13, 15, 19, 20 };
            if (shiftNoList.Any())
            {
                foreach (var shiftNo in shiftNoList)
                {
                    var strCon = context.Database.GetConnectionString();
                    var strSql = $@"select sum(amount) as amount
                                    from INF_POS_FUNCTION14 f4
                                    where SITE_ID = '52{request.BrnCode}'
                                    and convert(date,BUSINESS_DATE) = '{request.FromDate:yyyy-MM-dd}'
                                    and SHIFT_NO = {shiftNo}
                                    and MOP_CODE in ({string.Join(",", mopCode)})";

                    var amount = 0m;

                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = strSql;
                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {
                            while (result.Read())
                            {
                                var queryResult = result[0];
                                amount = queryResult == DBNull.Value ? 0 : Convert.ToDecimal(result[0]);
                            }
                        }

                        context.Database.CloseConnection();
                    }

                    if (amount > 0)
                    {
                        response.Amount += amount;
                    }
                    else
                    {
                        con.Open();
                        var sql = @$"select sum(pd.amt) amount
                                    from {schema}bill_header bh , raptorpos.payment_detail pd
                                    where  pd.com_code = bh.com_code
                                        and pd.branch_code = bh.branch_code
                                        and pd.pos_id = bh.pos_id
                                        and pd.pos_day_id = bh.pos_day_id
                                        and pd.shift_no = bh.shift_no
                                        and bh.business_date is not null
                                        and trunc(bh.BUSINESS_DATE) =  to_date('{request.FromDate:yyyy-MM-dd}','yyyy-mm-dd')  --> PARAMETER
                                        and bh.branch_at = '52{request.BrnCode}'  --> Parameter
                                        and bh.shift_no = {shiftNo}  --> Parameter
                                        and bh.bill_status <> '0002'
                                        and pd.pay_types in ({string.Join(",", mopCode)})
                                        and exists
                                        ( select null
                                            from raptorpos.payment_header ph
                                            where bh.com_code = ph.com_code
                                            and bh.branch_code = ph.branch_code
                                            and bh.pos_id = ph.pos_id
                                            and bh.pos_day_id = ph.pos_day_id
                                            and bh.shift_no = ph.shift_no
                                            and bh.sale_id = ph.sale_id
                                            and pd.payment_id = ph.payment_id)";
                        cmd = new OracleCommand
                        {
                            Connection = con,
                            CommandTimeout = 60,
                            CommandText = sql
                            //CommandText = $@"select sum(AMOUNT) AS amount
                            //                from {schema}FUNCTION14 f4
                            //                where SITE_ID = '52{request.BrnCode}'
                            //                and trunc(BUSINESS_DATE) = to_date('{request.FromDate:yyyy-MM-dd}','yyyy-mm-dd')  
                            //                and SHIFT_NO = {shiftNo}
                            //                and MOP_CODE IN (2,3,13,15,19,20)"
                        };


                        oracleDataReader = cmd.ExecuteReader();

                        while (oracleDataReader.Read())
                        {
                            var queryResult = oracleDataReader["amount"];
                            amount = queryResult == DBNull.Value ? 0 : Convert.ToDecimal(oracleDataReader["amount"]);
                            response.Amount += amount;
                        }

                        con.Close();
                        oracleDataReader.Close();
                    }
                }
            }

            oracleDataReader.Close();
            oracleDataReader.Dispose();
            con.Close();
            con.Dispose();

            return response;
        }

        private List<POSCash> GetPOSCashesFromSqlServer(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo, string policeTypeIds)
        {
            var posCashs = new List<POSCash>();
            string strSql = $@"SELECT journal_id,site_id,business_date,shift_no,taxinvno,user_card_no,total_goodsamt,total_discamt,total_taxamt,total_paid_amt,billno,plu_number,item_name,item_code,sell_price
                            ,sum(sell_qty) AS sell_qty,sum(goods_amt) AS goods_amt,sum(tax_amt) AS tax_amt,sum(disc_amt) AS disc_amt,sum(sum_item_amt) AS sum_item_amt,sum(sub_amt) AS sub_amt,sum(total_amt) AS  total_amt
                            from (
                                select f4.journal_id, f4.site_id, f4.business_date, f4.shift_no, f4.taxinvno,f4.user_card_no, f4.total_goodsamt, f4.total_discamt, f4.total_taxamt, f4.total_paid_amt, f4.BILLNO, f5.plu_number                                
                                , f5.sell_price, f5.sell_qty, f5.goods_amt
                                , f5.tax_amt , f5.disc_amt, '' as ITEM_CODE, f5.ITEM_NAME
                                , f5.goods_amt + f5.tax_amt + f5.disc_amt as sum_item_amt
                                , f5.goods_amt + f5.tax_amt  as sub_amt
                                , (f5.goods_amt *107/100) as total_amt
                                from INF_POS_FUNCTION4(nolock) f4, INF_POS_FUNCTION5(nolock) f5
                                where convert(varchar,f4.business_date,103) =  '{fromDate:dd/MM/yyyy}'
                                and f4.site_id = '52{brnCode}'
                                and (f5.disc_group = 'N' or (f5.disc_group = 'O' and f4.total_discamt <> 0))
                                and f5.PRODUCT_SUB_TYPE <> 'S'
                                and f4.journal_id = f5.journal_id
                                and f4.shift_no = {shiftNo}
                                and exists ( select null
                                    from INF_POS_FUNCTION14(nolock) f14
                                    where f14.mop_code in ({payTypeIds})
                                    and f14.journal_id = f4.journal_id
                                    and f14.site_id = f4.site_id )
                                and not exists ( select null
                                    from INF_POS_FUNCTION14(nolock) f14
                                    where f14.mop_code in ({policeTypeIds})
                                    and f14.journal_id = f4.journal_id
                                    and f14.site_id = f4.site_id )

                            )tbl
                            GROUP BY journal_id,site_id,business_date,shift_no,taxinvno,USER_CARD_NO,total_goodsamt,total_discamt,total_taxamt,total_paid_amt,billno,plu_number,item_name,item_code,sell_price
                        ";

            using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = strSql;
            context.Database.OpenConnection();

            // var masEmployees = context.MasEmployees.ToList();

            using (System.Data.Common.DbDataReader result = command.ExecuteReader())
            {
                while (result.Read())
                {

                    //var masEmployee = masEmployees.FirstOrDefault(x => x.EmpCode == empCode); 
                    var posCash = new POSCash();
                    posCash.JournalId = result["journal_id"].ToString() ?? string.Empty;
                    posCash.SiteId = result["site_id"].ToString() ?? string.Empty;
                    posCash.BusinessDate = (DateTime)result["business_date"];
                    posCash.ShiftNo = result["shift_no"].ToString() ?? string.Empty;
                    posCash.TaxInvNo = result["taxinvno"].ToString() ?? string.Empty;
                    //posCash.EmpCode = result["user_card_no"].ToString() ?? string.Empty;
                    posCash.EmpCode = result["user_card_no"].ToString() ?? string.Empty;
                    //posCash.EmpName = masEmployee != null ? masEmployee.EmpName : string.Empty;
                    posCash.TotalGoodAmt = Convert.ToDecimal(result["total_goodsamt"] ?? 0);
                    posCash.TotalDiscAmt = Convert.ToDecimal(result["total_discamt"] ?? 0);
                    posCash.TotalTaxAmt = Convert.ToDecimal(result["total_taxamt"] ?? 0);
                    posCash.TotalPaidAmt = Convert.ToDecimal(result["total_paid_amt"] ?? 0);
                    posCash.BillNo = result["billno"].ToString() ?? string.Empty;
                    posCash.PluNumber = result["plu_number"].ToString() ?? string.Empty;
                    posCash.ItemName = result["item_name"].ToString() ?? string.Empty;
                    posCash.ItemCode = result["item_code"].ToString() ?? string.Empty;
                    posCash.SellPrice = Convert.ToDecimal(result["sell_price"] ?? 0);
                    posCash.SellQty = Convert.ToDecimal(result["sell_qty"] ?? 0);
                    posCash.GoodsAmt = Convert.ToDecimal(result["goods_amt"] ?? 0);
                    posCash.TaxAmt = Convert.ToDecimal(result["tax_amt"] ?? 0);
                    posCash.DiscAmt = Convert.ToDecimal(result["disc_amt"] ?? 0);
                    posCash.SumItemAmt = Convert.ToDecimal(result["sum_item_amt"] ?? 0);
                    posCash.SubAmt = Convert.ToDecimal(result["sub_amt"] ?? 0);
                    posCash.TotalAmt = Convert.ToDecimal(result["total_amt"] ?? 0);
                    posCashs.Add(posCash);
                }
            }
            context.Database.CloseConnection();

            return posCashs;
        }

        private List<POSCash> GetPOSCashesFromOracle(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo, string payTypePolice)
        {
            var posCashs = new List<POSCash>();
            //var sqlConfigVersion = context.DopPosConfigs.FirstOrDefault(x => x.DocType == "Query");
            var commandText = string.Empty;

            commandText = GetCashSqlStringV2(fromDate, brnCode, payTypeIds, shiftNo, payTypePolice);
            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                #region Pagging
                //CommandText = $@"SELECT * FROM
                //                (
                //                    SELECT a.*, rownum r__
                //                    FROM
                //                    (
                //                        SELECT FUNCTION4.JOURNAL_ID, FUNCTION4.BILLNO, FUNCTION4.TOTAL_PAID_AMT, TOTAL_DISCAMT, FUNCTION4.TOTAL_GOODSAMT, FUNCTION4.TOTAL_TAXAMT
                //                        FROM FUNCTION4
                //                        LEFT JOIN FUNCTION5 ON  FUNCTION4.JOURNAL_ID = FUNCTION5.JOURNAL_ID
                //                        LEFT JOIN FUNCTION14 ON FUNCTION4.JOURNAL_ID = FUNCTION14.JOURNAL_ID
                //                        WHERE TO_CHAR(FUNCTION4.BUSINESS_DATE,'yyyymmdd') = '{query.FromDate:yyyyMMdd}' AND MOP_CODE = '1'
                //                        ORDER BY FUNCTION4.JOURNAL_ID ASC
                //                    ) a
                //                    WHERE rownum < (({query.Page} * {query.ItemsPerPage}) + 1 )
                //                )
                //                WHERE r__ >= ((({query.Page}-1) * {query.ItemsPerPage}) + 1)"
                #endregion

                Connection = con,
                CommandTimeout = 60,
                CommandText = commandText
            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();
            int intSeqNo = 1;
            //var masEmployees = context.MasEmployees.ToList();
            var masProducts = context.MasProducts.ToList();

            while (oracleDataReader.Read())
            {
                var pluNumber = oracleDataReader["PLU_NUMBER"].ToString();                
                var masproduct = masProducts.FirstOrDefault(x => x.PdId == pluNumber);
                //var masEmployee = masEmployees.FirstOrDefault(x => x.EmpCode == empCode);

                POSCash cash = new POSCash
                {
                    Row = intSeqNo++,
                    JournalId = oracleDataReader["JOURNAL_ID"].ToString() ?? string.Empty,
                    SiteId = oracleDataReader["SITE_ID"].ToString() ?? string.Empty,
                    BusinessDate = Convert.ToDateTime(oracleDataReader["BUSINESS_DATE"]),
                    ShiftNo = oracleDataReader["SHIFT_NO"].ToString() ?? string.Empty,
                    TaxInvNo = oracleDataReader["TAXINVNO"].ToString() ?? string.Empty,
                    //EmpCode = oracleDataReader["user_card_no"].ToString() ?? string.Empty,
                    EmpCode = oracleDataReader["user_card_no"].ToString() ?? string.Empty,
                    //EmpName = masEmployee != null ? masEmployee.EmpName : string.Empty,
                    TotalGoodAmt = Convert.ToDecimal(oracleDataReader["TOTAL_GOODSAMT"] ?? 0),
                    TotalDiscAmt = Convert.ToDecimal(oracleDataReader["TOTAL_DISCAMT"] ?? 0),
                    TotalTaxAmt = Convert.ToDecimal(oracleDataReader["TOTAL_TAXAMT"] ?? 0),
                    TotalPaidAmt = Convert.ToDecimal(oracleDataReader["TOTAL_PAID_AMT"] ?? 0),
                    PluNumber = oracleDataReader["PLU_NUMBER"].ToString() ?? string.Empty,
                    SellQty = Convert.ToDecimal(oracleDataReader["SELL_QTY"] ?? 0),
                    SellPrice = Convert.ToDecimal(oracleDataReader["SELL_PRICE"] ?? 0),
                    GoodsAmt = Convert.ToDecimal(oracleDataReader["GOODS_AMT"] ?? 0),
                    TaxAmt = Convert.ToDecimal(oracleDataReader["TAX_AMT"] ?? 0),
                    DiscAmt = Convert.ToDecimal(oracleDataReader["DISC_AMT"] ?? 0),
                    ItemCode = masproduct != null ? masproduct.PdId : string.Empty,
                    ItemName = masproduct != null ? masproduct.PdName : string.Empty,
                    BillNo = oracleDataReader["JOURNAL_ID"].ToString() ?? string.Empty,
                    SumItemAmt = Convert.ToDecimal(oracleDataReader["SUM_ITEM_AMT"] ?? 0),
                    SubAmt = Convert.ToDecimal(oracleDataReader["SUB_AMT"] ?? 0),
                    TotalAmt = Convert.ToDecimal(oracleDataReader["TOTAL_AMT"] ?? 0),
                };

                posCashs.Add(cash);
            }

            con.Close();
            oracleDataReader.Close();
            return posCashs.OrderBy(x => x.JournalId).ToList();
        }

        private string GetCashSqlStringV1(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo)
        {
            return $@"select f4.journal_id, f4.site_id, f4.business_date, f4.shift_no, f4.taxinvno, f4.total_goodsamt, f4.total_discamt, f4.total_taxamt, f4.total_paid_amt, f4.BILLNO
                                , f5.plu_number, f5.sell_qty, f5.sell_price, f5.goods_amt, f5.tax_amt , f5.disc_amt, f5.ITEM_CODE, f5.ITEM_NAME
                                ,f5.goods_amt + f5.tax_amt + f5.disc_amt as sum_item_amt
                                ,f5.goods_amt + f5.tax_amt  as sub_amt
                                ,(f5.goods_amt *107/100) as total_amt
                                from {schema}function4 f4, {schema}function5 f5
                                where trunc(f4.business_date) = to_date('{fromDate:dd/MM/yyyy}','dd/MM/yyyy')
                                and f4.site_id = '52{brnCode}'
                                and (f5.disc_group = 'N' or (f5.disc_group = 'O' and f5.disc_amt <> 0))
                                and exists
                                ( select null 
                                from {schema}function14 f14
                                where f14.mop_code in ({payTypeIds})
                                and f14.journal_id = f4.journal_id
                                and f14.site_id = f4.site_id )
                                and f4.journal_id = f5.journal_id
                                and f4.shift_no = {shiftNo}";
        }

        private string GetCashSqlStringV2(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo, string payTypePolice)
        {
            return $@"
                SELECT journal_id,site_id,business_date,shift_no,taxinvno,user_card_no,total_goodsamt,total_discamt,total_taxamt,total_paid_amt,billno,plu_number,item_name,sell_price
                ,sum(sell_qty) AS sell_qty,sum(goods_amt) AS goods_amt,sum(tax_amt) AS tax_amt,sum(disc_amt) AS disc_amt,sum(sum_item_amt) AS sum_item_amt,sum(sub_amt) AS sub_amt,sum(total_amt) AS  total_amt
                FROM (
                        select a.bill_no journal_id, a.branch_at site_id, trunc(a.business_date) business_date
                      , a.shift_no, '' taxinvno,a.user_card_no, (a.total_amt - a.total_vatamt) - a.total_discount total_goodsamt
                      , a.total_discount total_discamt, a.total_vatamt total_taxamt, a.total_amt total_paid_amt, a.bill_no billno
                      , c.product_code plu_number
                      , c.product_desc item_name
                      , b.sell_qty sell_qty
                      , b.unit_amt sell_price
                      , b.netamt - b.vatamt goods_amt
                      , b.vatamt tax_amt
                      , b.discount disc_amt
                      , b.netamt + b.discount sum_item_amt
                      , b.netamt sub_amt
                      , ((b.netamt - b.vatamt) *107/100) as total_amt
                    from raptorpos.bill_header a, raptorpos.bill_detail b, raptorpos.product c
                    where a.com_code = b.com_code
                        and a.branch_code = b.branch_code
                        and a.pos_id = b.pos_id
                        and a.pos_day_id = b.pos_day_id
                        and a.shift_no = b.shift_no
                        and a.bill_id = b.bill_id
                        and b.ref_t_product_code = c.product_code
                        and trunc(a.BUSINESS_DATE) = TO_DATE('{fromDate:dd/MM/yyyy}','dd/mm/yyyy')  -- parameter											
                        and a.shift_no = {shiftNo} 		 -- parameter									
                        and a.BRANCH_AT  = '52{brnCode}'   -- parameter		
                        and a.bill_status <> '0002'
                        and a.business_date is not null  
                        and c.product_type = 'N'  -- nonoil                        
                        AND (b.PRODUCT_SUB_TYPE  <> 'S' OR b.PRODUCT_SUB_TYPE  IS NULL)
                        and exists ( select null
                            from raptorpos.m_desc m
                            where a.bill_status = m.desc_code
                            and m.desc_parent_id = '2060')
                      and exists
                        ( select null
                          from (
                                select hd.sale_id, hd.com_code, hd.branch_at, hd.pos_id, hd.pos_day_id, hd.shift_no
                                from raptorpos.payment_header hd, raptorpos.payment_detail dt
                                where hd.branch_at = '52{brnCode}'    -- parameter		
                                and hd.shift_no  = {shiftNo} 		    -- parameter	
                                and dt.pay_types in  ({payTypeIds})     -- parameter		
                                and hd.com_code = dt.com_code 
                                and hd.branch_code = dt.branch_code
                                and hd.pos_id = dt.pos_id
                                and hd.pos_day_id = dt.pos_day_id
                                and hd.shift_no = dt.shift_no
                                and hd.payment_id = dt.payment_id ) p      
                          where a.com_code = p.com_code
                            and a.branch_at = p.branch_at
                            and a.pos_id = p.pos_id
                            and a.pos_day_id = p.pos_day_id
                            and a.shift_no = p.shift_no
                            and a.sale_id = p.sale_id )               
                    union ALL
                    select a.bill_no journal_id, a.branch_at site_id, trunc(a.business_date) business_date
                      , a.shift_no, '' taxinvno, a.user_card_no, (a.total_amt - a.total_vatamt) - a.total_discount total_goodsamt
                      , a.total_discount total_discamt, a.total_vatamt total_taxamt, a.total_amt total_paid_amt, a.bill_no billno
                      , c.product_code plu_number
                      , c.product_desc item_name
                      , b.sell_qty sell_qty
                      , b.unit_amt sell_price
                      , b.netamt - b.vatamt goods_amt
                      , b.vatamt tax_amt
                      , b.discount disc_amt
                      , b.netamt + b.discount sum_item_amt
                      , b.netamt sub_amt
                      , ((b.netamt - b.vatamt) *107/100) as total_amt
                    from raptorpos.bill_header a, raptorpos.bill_detail b, raptorpos.product c
                    where a.com_code = b.com_code
                      and a.branch_code = b.branch_code
                      and a.pos_id = b.pos_id
                      and a.pos_day_id = b.pos_day_id
                      and a.shift_no = b.shift_no
                      and a.bill_id = b.bill_id
                      and b.ref_t_product_code = c.product_code
                      AND trunc(a.BUSINESS_DATE) = TO_DATE('{fromDate:dd/MM/yyyy}','dd/mm/yyyy')  -- parameter	
                      and a.shift_no = {shiftNo} 	     -- parameter	
                      AND a.BRANCH_AT = '52{brnCode}'   -- parameter		
                      and a.bill_status <> '0002'
                      and a.business_date is not null  
                      and c.product_type = 'O' 
                      AND a.TOTAL_DISCOUNT  <> 0
                      and exists
                        ( select null
                          from raptorpos.m_desc m
                          where a.bill_status = m.desc_code
                            and m.desc_parent_id = '2060')     
                     and not exists
                        ( select null
                          from (
                                select hd.sale_id, hd.com_code, hd.branch_at, hd.pos_id, hd.pos_day_id, hd.shift_no
                                from raptorpos.payment_header hd, raptorpos.payment_detail dt
                                where hd.branch_at = '52{brnCode}'   -- parameter	
                                and hd.shift_no = {shiftNo} 	     -- parameter	
                                and dt.pay_types  in ({payTypePolice})       -- parameter
                                and hd.com_code = dt.com_code 
                                and hd.branch_code = dt.branch_code
                                and hd.pos_id = dt.pos_id
                                and hd.pos_day_id = dt.pos_day_id
                                and hd.shift_no = dt.shift_no
                                and hd.payment_id = dt.payment_id ) p      
                          where a.com_code = p.com_code
                            and a.branch_at = p.branch_at
                            and a.pos_id = p.pos_id
                            and a.pos_day_id = p.pos_day_id
                            and a.shift_no = p.shift_no
                            and a.sale_id = p.sale_id
	                    )
                )tbl
                GROUP BY journal_id,site_id,business_date,shift_no,taxinvno,user_card_no,total_goodsamt,total_discamt,total_taxamt,total_paid_amt,billno,plu_number,item_name,sell_price
                ";
        }

        //private string GetCashSqlStringV2(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo)
        //{
        //    return $@"select a.bill_no journal_id, a.branch_at site_id, trunc(a.business_date) business_date											
        //                      , a.shift_no, '' taxinvno, (a.total_amt - a.total_vatamt) - a.total_discount total_goodsamt											
        //                      , a.total_discount total_discamt, a.total_vatamt total_taxamt, a.total_amt total_paid_amt, a.bill_no billno											
        //                      , c.product_code plu_number											
        //                      , c.product_desc item_name											
        //                      , b.sell_qty sell_qty											
        //                      , b.unit_amt sell_price											
        //                      , b.netamt - b.vatamt goods_amt											
        //                      , b.vatamt tax_amt											
        //                      , b.discount disc_amt											
        //                      , b.netamt + b.discount sum_item_amt											
        //                      , b.netamt sub_amt											
        //                      , ((b.netamt - b.vatamt) *107/100) as total_amt											
        //                    from raptorpos.bill_header a, raptorpos.bill_detail b, raptorpos.product c											
        //                    where a.com_code = b.com_code											
        //                      and a.branch_code = b.branch_code											
        //                      and a.pos_id = b.pos_id											
        //                      and a.pos_day_id = b.pos_day_id											
        //                      and a.shift_no = b.shift_no											
        //                      and a.bill_id = b.bill_id											
        //                      and b.ref_t_product_code = c.product_code											
        //                      AND trunc(a.BUSINESS_DATE) = TO_DATE('{fromDate:dd/MM/yyyy}','dd/mm/yyyy')  -- parameter											
        //                      and a.shift_no = {shiftNo} 		 -- parameter									
        //                      AND a.BRANCH_AT  = '52{brnCode}'   -- parameter											
        //                      and a.bill_status <> '0002'											
        //                      and a.business_date is not null  											
        //                      and c.product_type = 'N'											
        //                      and exists											
        //                        ( select null											
        //                          from raptorpos.m_desc m											
        //                          where a.bill_status = m.desc_code											
        //                            and m.desc_parent_id = '2060')											
        //                      and exists											
        //                        ( select null											
        //                          from raptorpos.payment_detail p											
        //                          where a.com_code = p.com_code											
        //                            and a.branch_code = p.branch_code											
        //                            and a.pos_id = p.pos_id											
        //                            and a.pos_day_id = p.pos_day_id											
        //                            and a.shift_no = p.shift_no											
        //                            and p.pay_types in ({payTypeIds}))											
        //                    union all											
        //                    select a.bill_no journal_id, a.branch_at site_id, trunc(a.business_date) business_date											
        //                      , a.shift_no, '' taxinvno, (a.total_amt - a.total_vatamt) - a.total_discount total_goodsamt											
        //                      , a.total_discount total_discamt, a.total_vatamt total_taxamt, a.total_amt total_paid_amt, a.bill_no billno											
        //                      , c.product_code plu_number											
        //                      , c.product_desc item_name											
        //                      , b.sell_qty sell_qty											
        //                      , b.unit_amt sell_price											
        //                      , b.netamt - b.vatamt goods_amt											
        //                      , b.vatamt tax_amt											
        //                      , b.discount disc_amt											
        //                      , b.netamt + b.discount sum_item_amt											
        //                      , b.netamt sub_amt											
        //                      , ((b.netamt - b.vatamt) *107/100) as total_amt											
        //                    from raptorpos.bill_header a, raptorpos.bill_detail b, raptorpos.product c											
        //                    where a.com_code = b.com_code											
        //                      and a.branch_code = b.branch_code											
        //                      and a.pos_id = b.pos_id											
        //                      and a.pos_day_id = b.pos_day_id											
        //                      and a.shift_no = b.shift_no											
        //                      and a.bill_id = b.bill_id											
        //                      and b.ref_t_product_code = c.product_code											
        //                      AND trunc(a.BUSINESS_DATE) = TO_DATE('{fromDate:dd/MM/yyyy}','dd/mm/yyyy')    -- parameter											
        //                      and a.shift_no = {shiftNo} 				 -- parameter							
        //                      AND a.BRANCH_AT  = '52{brnCode}'		 -- parameter									
        //                      and a.bill_status <> '0002'											
        //                      and a.business_date is not null  											
        //                      and c.product_type = 'O'											
        //                      and b.discount <> 0											
        //                      and exists											
        //                        ( select null											
        //                          from raptorpos.m_desc m											
        //                          where a.bill_status = m.desc_code											
        //                            and m.desc_parent_id = '2060')											
        //                      and exists											
        //                        ( select null											
        //                          from raptorpos.payment_detail p											
        //                          where a.com_code = p.com_code											
        //                            and a.branch_code = p.branch_code											
        //                            and a.pos_id = p.pos_id											
        //                            and a.pos_day_id = p.pos_day_id											
        //                            and a.shift_no = p.shift_no		
        //                     )";
        //}

        private List<POSCredit> GetPOSCreditsFromSqlServer(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo)
        {
            var posCredits = new List<POSCredit>();
            string strSql = $@"select f4.journal_id, f4.site_id, f4.business_date, f4.shift_no, f4.taxinvno,f4.USER_CARD_NO, f4.total_goodsamt, f4.total_discamt, f4.total_taxamt, f4.total_paid_amt
                             , f4.customer_id, f4.lic_no, f4.miles, f4.billno  
                             , f5.plu_number
                             , f5.sell_qty
                             , f5.sell_price
                             , f5.goods_amt
                             , f5.tax_amt 
                             , f5.disc_amt  
                             , f14.amount
                             , f14.pono 
                             ,f5.goods_amt + f5.tax_amt + f5.disc_amt as sum_item_amt
                             ,f5.goods_amt + f5.tax_amt  as sub_amt
                             ,(f5.goods_amt *107/100) as total_amt
                            from INF_POS_FUNCTION4(nolock) f4, INF_POS_FUNCTION5(nolock) f5
                            ,(
                                select journal_id, site_id, business_date, shift_no, mop_info, mop_code, amount, pono
                                from INF_POS_FUNCTION14(nolock) 
                                where convert(varchar, business_date,103) =  '{fromDate:dd/MM/yyyy}'
                                and site_id = '52{brnCode}'
                                and mop_code in ({payTypeIds})
                            ) f14
                            where f5.disc_group <> 'F'
                            and f5.PRODUCT_SUB_TYPE <> 'S'
                            and f4.journal_id = f5.journal_id
                            and f4.journal_id = f14.journal_id
                            and f4.site_id = f14.site_id
                            and convert(date,f4.business_date,103) = convert(date,f14.business_date,103)
                            and f4.shift_no = {shiftNo}";

            var products = context.MasProducts.ToList();
            var companies = context.MasCompanies.ToList();
            var customerCars = context.MasCustomerCars.ToList();
            //var employees = context.MasEmployees.ToList();

            using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = strSql;
            context.Database.OpenConnection();

            using (System.Data.Common.DbDataReader result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    var pluNumber = result["plu_number"].ToString() ?? string.Empty;
                    var custCode = result["customer_id"].ToString() ?? string.Empty;
                    //var empCode = result["USER_CARD_NO"].ToString() ?? string.Empty;
                    var masproduct = products.FirstOrDefault(x => x.PdId == pluNumber); //context.MasProducts.FirstOrDefault(x => x.PdId == pluNumber);
                    var customerCompany = companies.FirstOrDefault(x => x.CustCode == custCode);
                    //var employee = employees.FirstOrDefault(x => x.EmpCode == empCode);
                    var posCredit = new POSCredit();

                    if (customerCompany != null)
                    {
                        var customerCompanies = customerCars.Where(x => x.CustCode == custCode).Select(x => x.LicensePlate).ToList();

                        if (customerCompanies != null)
                        {
                            posCredit.LicensePlates = customerCompanies;
                            posCredit.IsCustomerCompany = true;
                        }
                    }

                    posCredit.JournalId = result["journal_id"].ToString() ?? string.Empty;
                    posCredit.SiteId = result["site_id"].ToString() ?? string.Empty;
                    posCredit.BusinessDate = (DateTime)result["business_date"];
                    posCredit.ShiftNo = result["shift_no"].ToString() ?? string.Empty;
                    posCredit.TaxInvNo = result["taxinvno"].ToString() ?? string.Empty;
                    //posCredit.EmpCode = result["USER_CARD_NO"].ToString() ?? string.Empty;
                    posCredit.EmpCode = result["USER_CARD_NO"].ToString() ?? string.Empty;
                    //posCredit.EmpName = employee != null ? employee.EmpName : string.Empty;
                    posCredit.TotalGoodsAmt = Convert.ToDecimal(result["total_goodsamt"] ?? 0);
                    posCredit.TotalDiscAmt = Convert.ToDecimal(result["total_discamt"] ?? 0);
                    posCredit.TotalTaxAmt = Convert.ToDecimal(result["total_taxamt"] ?? 0);
                    posCredit.TotalPaidAmt = Convert.ToDecimal(result["total_paid_amt"] ?? 0);
                    posCredit.CustomerId = custCode;
                    posCredit.LicNo = result["lic_no"].ToString() ?? string.Empty;
                    posCredit.Miles = result["miles"].ToString() ?? string.Empty;
                    posCredit.BillNo = result["billno"].ToString() ?? string.Empty;
                    posCredit.ItemCode = masproduct != null ? masproduct.PdId : string.Empty;
                    posCredit.ItemName = masproduct != null ? masproduct.PdName : string.Empty;
                    posCredit.PluNumber = result["plu_number"].ToString() ?? string.Empty;
                    posCredit.SellQty = Convert.ToDecimal(result["sell_qty"] ?? 0);
                    posCredit.SellPrice = Convert.ToDecimal(result["sell_price"] ?? 0);
                    posCredit.GoodsAmt = Convert.ToDecimal(result["goods_amt"] ?? 0);
                    posCredit.TaxAmt = Convert.ToDecimal(result["tax_amt"] ?? 0);
                    posCredit.DiscAmt = Convert.ToDecimal(result["disc_amt"] ?? 0);
                    posCredit.Amount = Convert.ToDecimal(result["amount"] ?? 0);
                    posCredit.Po = result["pono"].ToString() ?? string.Empty;
                    posCredit.SumItemAmt = Convert.ToDecimal(result["sum_item_amt"] ?? 0);
                    posCredit.SubAmt = Convert.ToDecimal(result["sub_amt"] ?? 0);
                    posCredit.TotalAmt = Convert.ToDecimal(result["total_amt"] ?? 0);
                    posCredits.Add(posCredit);
                }
            }


            context.Database.CloseConnection();

            return posCredits;
        }

        private List<POSCredit> GetPOSCreditsFromOracle(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo)
        {
            var posCredits = new List<POSCredit>();
            //var sqlConfigVersion = context.DopPosConfigs.FirstOrDefault(x => x.DocType == "Query");
            var commandText = string.Empty;

            commandText = GetCreditSqlStringV2(fromDate, brnCode, payTypeIds, shiftNo);

            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 60,
                CommandText = commandText
            };

            var products = context.MasProducts.ToList();
            var companies = context.MasCompanies.ToList();
            var customerCars = context.MasCustomerCars.ToList();
            //var employees = context.MasEmployees.ToList();

            OracleDataReader oracleDataReader = cmd.ExecuteReader();
            while (oracleDataReader.Read())
            {
                var custCode = oracleDataReader["CUSTOMER_ID"].ToString() ?? string.Empty;
                var pluNumber = oracleDataReader["PLU_NUMBER"].ToString();
                //var empCode = oracleDataReader["USER_CARD_NO"].ToString()?? string.Empty;
                var masproduct = products.FirstOrDefault(x => x.PdId == pluNumber);
                var customerCompany = companies.FirstOrDefault(x => x.CustCode == custCode);
                //var employee = employees.FirstOrDefault(x => x.EmpCode == empCode);


                POSCredit posCredit = new POSCredit();

                if (customerCompany != null)
                {
                    var customerCompanies = customerCars.Where(x => x.CustCode == custCode).Select(x => x.LicensePlate).ToList();

                    if (customerCompanies != null)
                    {
                        posCredit.LicensePlates = customerCompanies;
                        posCredit.IsCustomerCompany = true;
                    }
                }
                posCredit.JournalId = oracleDataReader["JOURNAL_ID"].ToString() ?? string.Empty;
                posCredit.SiteId = oracleDataReader["SITE_ID"].ToString() ?? string.Empty;
                posCredit.BusinessDate = Convert.ToDateTime(oracleDataReader["BUSINESS_DATE"]);
                posCredit.ShiftNo = oracleDataReader["SHIFT_NO"].ToString() ?? string.Empty;
                posCredit.TaxInvNo = oracleDataReader["TAXINVNO"].ToString() ?? string.Empty;
                //posCredit.EmpCode = oracleDataReader["USER_CARD_NO"].ToString() ?? string.Empty;
                posCredit.EmpCode = oracleDataReader["USER_CARD_NO"].ToString() ?? string.Empty;
                //posCredit.EmpName = employee != null ? employee.EmpName : string.Empty;
                posCredit.TotalGoodsAmt = Convert.ToDecimal(oracleDataReader["TOTAL_GOODSAMT"] ?? 0);
                posCredit.TotalDiscAmt = Convert.ToDecimal(oracleDataReader["TOTAL_DISCAMT"] ?? 0);
                posCredit.TotalTaxAmt = Convert.ToDecimal(oracleDataReader["TOTAL_TAXAMT"] ?? 0);
                posCredit.TotalPaidAmt = Convert.ToDecimal(oracleDataReader["TOTAL_PAID_AMT"] ?? 0);
                posCredit.PluNumber = oracleDataReader["PLU_NUMBER"].ToString() ?? string.Empty;
                posCredit.SellQty = Convert.ToDecimal(oracleDataReader["SELL_QTY"] ?? 0);
                posCredit.SellPrice = Convert.ToDecimal(oracleDataReader["SELL_PRICE"] ?? 0);
                posCredit.GoodsAmt = Convert.ToDecimal(oracleDataReader["GOODS_AMT"] ?? 0);
                posCredit.TaxAmt = Convert.ToDecimal(oracleDataReader["TAX_AMT"] ?? 0);
                posCredit.DiscAmt = Convert.ToDecimal(oracleDataReader["DISC_AMT"] ?? 0);
                posCredit.ItemCode = masproduct != null ? masproduct.PdId : string.Empty;
                posCredit.ItemName = masproduct != null ? masproduct.PdName : string.Empty;
                posCredit.CustomerId = custCode;
                posCredit.LicNo = oracleDataReader["LIC_NO"].ToString().Trim() ?? string.Empty;
                posCredit.Miles = oracleDataReader["MILES"].ToString() ?? string.Empty;
                posCredit.Po = oracleDataReader["PONO"].ToString() ?? string.Empty;
                posCredit.BillNo = oracleDataReader["JOURNAL_ID"].ToString() ?? string.Empty;
                posCredit.Amount = Convert.ToDecimal(oracleDataReader["AMOUNT"] ?? 0);
                posCredit.SumItemAmt = Convert.ToDecimal(oracleDataReader["SUM_ITEM_AMT"] ?? 0);
                posCredit.SubAmt = Convert.ToDecimal(oracleDataReader["SUB_AMT"] ?? 0);
                posCredit.TotalAmt = Convert.ToDecimal(oracleDataReader["TOTAL_AMT"] ?? 0);
                posCredits.Add(posCredit);
            }

            con.Close();
            oracleDataReader.Close();

            return posCredits;
        }

        //private string GetCreditSqlStringV1(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo)
        //{
        //    return $@"select f4.journal_id, f4.site_id, f4.business_date, f4.shift_no, f4.taxinvno, f4.total_goodsamt, f4.total_discamt, f4.total_taxamt, f4.total_paid_amt
        //                , f4.customer_id, f4.lic_no, f4.miles, f4.billno  
        //                , f5.plu_number
        //                , f5.sell_qty
        //                , f5.sell_price
        //                , f5.goods_amt
        //                , f5.tax_amt 
        //                , f5.disc_amt  
        //                , f14.amount
        //                , f14.pono 
        //                ,f5.goods_amt + f5.tax_amt + f5.disc_amt as sum_item_amt
        //                ,f5.goods_amt + f5.tax_amt  as sub_amt
        //                ,(f5.goods_amt *107/100) as total_amt
        //            from {schema}function4 f4, {schema}function5 f5
        //            ,(
        //                select journal_id, site_id, business_date, shift_no, mop_info, mop_code, amount, pono
        //                from {schema}function14 
        //                where trunc(business_date) = to_date('{fromDate:dd/MM/yyyy}','dd/MM/yyyy') --parameter: วันที่โหลดข้อมูล
        //                and site_id = '52{brnCode}' --parameter: branch
        //                and mop_code in ({payTypeIds}) --parameter : pay_type_id 
        //            ) f14
        //            where f5.disc_group <> 'F'
        //            and f4.journal_id = f5.journal_id
        //            and f4.journal_id = f14.journal_id
        //            and f4.site_id = f14.site_id
        //            and f4.business_date = trunc(f14.business_date)
        //            and f4.shift_no = {shiftNo}";
        //}

        private string GetCreditSqlStringV2(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo)
        {
            return $@"select a.bill_no journal_id, a.branch_at site_id, trunc(a.business_date) business_date                                            																							
                              , a.shift_no, '' taxinvno, a.USER_CARD_NO, (a.total_amt - a.total_vatamt) - a.total_discount total_goodsamt                                            																							
                              , a.total_discount total_discamt, a.total_vatamt total_taxamt, a.total_amt total_paid_amt                                            																							
                              , e.custno customer_id, e.licenseno lic_no, decode(e.meter, 'N', '0', e.meter) miles                                            																							
                              , a.bill_no billno                                            																							
                              , c.product_code plu_number                                            																							
                              , b.sell_qty sell_qty                                            																							
                              , b.unit_amt sell_price                                            																							
                              , b.netamt - b.vatamt goods_amt                                            																							
                              , b.vatamt tax_amt                                            																							
                              , b.discount disc_amt                                            																							
                              , d.amt amount                                           																					
                              , d.pono pono                                            																							
                              , b.netamt + b.discount sum_item_amt                                            																							
                              , b.netamt sub_amt                                            																							
                              , ((b.netamt - b.vatamt) * 107/100) as total_amt                                																							
                            from raptorpos.bill_header a, raptorpos.bill_detail b, raptorpos.product c                                            																							
                              , raptorpos.payment_detail d, raptorpos.paycashcredit e                                            																							
                            where a.com_code = b.com_code                                            																							
                              and a.branch_code = b.branch_code                                            																							
                              and a.pos_id = b.pos_id                                            																							
                              and a.pos_day_id = b.pos_day_id                                            																							
                              and a.shift_no = b.shift_no                                            																							
                              and a.bill_id = b.bill_id                                            																							
                              and b.ref_t_product_code = c.product_code                                             																							
                              and a.com_code = d.com_code                                            																							
                              and a.branch_code = d.branch_code                                            																							
                              and a.pos_id = d.pos_id                                            																							
                              and a.pos_day_id = d.pos_day_id                                            																							
                              and a.shift_no = d.shift_no                                               																							
                              and a.bill_id = e.ref_billid(+)                                            																							
                              and a.pos_id = e.pos_id(+)                                            																							
                              and a.branch_code = e.branch_code(+)                                            																							
                              and a.com_code = e.com_code(+)                                            																							
                              and a.shift_no = e.shift_no(+)                                            																							
                              and a.pos_day_id = e.pos_day_id(+)                                                                                    																							
                              and c.product_type <> 'F'  
                              AND (b.PRODUCT_SUB_TYPE  <> 'S' OR b.PRODUCT_SUB_TYPE  IS NULL)
                              and d.pay_types in ({payTypeIds})                                                																							
                              and a.bill_status <> '0002'                                																							
                              and trunc(a.business_date) = TO_DATE('{fromDate:dd/MM/yyyy}','dd/mm/yyyy')  --> Parameter                                																							
                              and a.shift_no = {shiftNo}             	--> Parameter               																						
                              and a.branch_at = '52{brnCode}' -- Parameter                                																							
                              and exists                                            																							
                                ( select null                                            																							
                                  from raptorpos.m_desc m                                            																							
                                  where a.bill_status = m.desc_code                                            																							
                                    and m.desc_parent_id = '2060')                                       																							
                              and exists                                																							
                                ( select null                                																							
                                  from raptorpos.s_transaction_detail trd                                																							
                                  where a.com_code = trd.com_code                                																							
                                    and a.branch_code = trd.branch_code                                																							
                                    and a.pos_id = trd.pos_id                                																							
                                    and a.pos_day_id = trd.pos_day_id                                																							
                                    and a.shift_no = trd.shift_no                                																							
                                    and a.sale_id = trd.sale_id                                																							
                                    and b.branch_code = trd.branch_code                                																							
                                    and b.pos_id = trd.pos_id                                																							
                                    and b.pos_day_id = trd.pos_day_id                                																							
                                    and b.shift_no = trd.shift_no                                																							
                                    and b.ref_t_trans_id = trd.ref_t_trans_id                                																							
                                    and b.item_id = trd.sale_item_id                                																							
                                    and trd.itemtype in ('O', 'N', 'F'))																							
                              and exists                                																							
                                ( select null                                																							
                                  from raptorpos.payment_header ph                                																							
                                  where a.com_code = ph.com_code                                																							
                                    and a.branch_code = ph.branch_code                                																							
                                    and a.pos_id = ph.pos_id																							
                                    and a.pos_day_id = ph.pos_day_id                                																							
                                    and a.shift_no = ph.shift_no                                																							
                                    and a.sale_id = ph.sale_id                                																							
                                    and d.payment_id = ph.payment_id)";
        }

        private List<POSReceive> GetPOSReceivesFromSqlServer(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo, string policeTypeIds)
        {
            var custCode = this.context.DopPosConfigs.FirstOrDefault(x => x.DocType == "Receive" && x.DocDesc == "Customer");

            var posReceives = new List<POSReceive>();
            string strSql = $@"
                    select f4.journal_id, f4.site_id, f4.business_date, f4.shift_no,f5.plu_number, f5.item_name , f5.sell_qty , f5.sell_price, f5.goods_amt    , f5.tax_amt , f5.disc_amt                      
                    , f5.goods_amt + f5.tax_amt + f5.disc_amt as sum_item_amt
                    , f5.goods_amt + f5.tax_amt  as sub_amt
                    , (f5.goods_amt *107/100) as total_amt
                    from INF_POS_FUNCTION4(nolock) f4, INF_POS_FUNCTION5(nolock) f5
                    where convert(varchar,f4.business_date,103) =   '{fromDate:dd/MM/yyyy}'	-- parameter : doc_date
                    and f4.site_id =  '52{brnCode}'		 -- parameter : brn_code
                    and f5.disc_group = 'N'
                    AND f5.PRODUCT_SUB_TYPE  = 'S'
                    and f4.journal_id = f5.journal_id
                    and f4.shift_no = {shiftNo}			 -- parameter : period_no
                    and exists ( select null
		                    from INF_POS_FUNCTION14(nolock) f14
		                    where f14.mop_code  in ({payTypeIds}) -- paytypeid : cash
		                    and f14.journal_id = f4.journal_id
		                    and f14.site_id = f4.site_id )
                    and not exists ( select null
		                    from INF_POS_FUNCTION14(nolock) f14
		                    where f14.mop_code in  ({policeTypeIds})  -- parameter : paytypeid => police
		                    and f14.journal_id = f4.journal_id
		                    and f14.site_id = f4.site_id )";

            using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = strSql;
            context.Database.OpenConnection();

            using (System.Data.Common.DbDataReader result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    var posReceive = new POSReceive();
                    posReceive.CustCode = custCode == null ? "" : custCode.PdId;
                    posReceive.JournalId = result[0].ToString() ?? string.Empty;
                    posReceive.SiteId = result[1].ToString() ?? string.Empty;
                    posReceive.BusinessDate = (DateTime)result[2];
                    posReceive.ShiftNo = result[3].ToString() ?? string.Empty;
                    posReceive.PluNumber = result[4].ToString() ?? string.Empty;
                    posReceive.ItemName = result[5].ToString() ?? string.Empty;
                    posReceive.SellQty = Convert.ToDecimal(result[6] ?? 0);
                    posReceive.SellPrice = Convert.ToDecimal(result[7] ?? 0);
                    posReceive.GoodsAmt = Convert.ToDecimal(result[8] ?? 0);
                    posReceive.TaxAmt = Convert.ToDecimal(result[9] ?? 0);
                    posReceive.DiscAmt = Convert.ToDecimal(result[10] ?? 0);
                    posReceive.SumItemAmt = Convert.ToDecimal(result[11] ?? 0);
                    posReceive.SubAmt = Convert.ToDecimal(result[12] ?? 0);
                    posReceive.TotalAmt = Convert.ToDecimal(result[13] ?? 0);
                    posReceives.Add(posReceive);
                }
            }
            context.Database.CloseConnection();

            return posReceives;
        }

        private List<POSReceive> GetPOSReceivesFromOracle(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo, string payTypePolice)
        {
            var posReceives = new List<POSReceive>();
            var commandText = string.Empty;
            var custCode = this.context.DopPosConfigs.FirstOrDefault(x => x.DocType == "Receive" && x.DocDesc == "Customer");
            //commandText = GetReceiveSqlString(fromDate, brnCode, payTypeIds, shiftNo, payTypePolice);

            commandText = $@"
                            select a.bill_no journal_id, a.branch_at site_id, trunc(a.BUSINESS_DATE) business_date
                            , a.shift_no
                            , c.product_code plu_number
                            , c.product_desc item_name
                            , b.sell_qty sell_qty
                            , b.unit_amt sell_price
                            , b.netamt - b.vatamt goods_amt
                            , b.vatamt tax_amt
                            , b.discount disc_amt
                            , b.netamt + b.discount sum_item_amt
                            , b.netamt sub_amt
                            , ((b.netamt - b.vatamt) *107/100) as total_amt
                            from raptorpos.bill_header a, raptorpos.bill_detail b, raptorpos.product c
                            where a.com_code = b.com_code
                            and a.branch_code = b.branch_code
                            and a.pos_id = b.pos_id
                            and a.pos_day_id = b.pos_day_id
                            and a.shift_no = b.shift_no
                            and a.bill_id = b.bill_id
                            and b.ref_t_product_code = c.product_code
                            and trunc(a.BUSINESS_DATE) = TO_DATE('{fromDate:dd/MM/yyyy}','dd/mm/yyyy') --> Parameter
                            and a.shift_no IN ({shiftNo})           -- parameter
                            and a.BRANCH_AT = '52{brnCode}'       -- parameter
                            and a.bill_status <> '0002'     
                            and a.business_date is not null
                            and c.product_type = 'N' -- nonoil
                            AND b.PRODUCT_SUB_TYPE  = 'S'
                            and exists ( select null
                            from raptorpos.m_desc m
                            where a.bill_status = m.desc_code
                            and m.desc_parent_id = '2060')
                            and exists
                            ( select null
                            from (
                            select hd.sale_id, hd.com_code, hd.branch_code, hd.pos_id, hd.pos_day_id, hd.shift_no
                            from raptorpos.payment_header hd, raptorpos.payment_detail dt
                            where hd.BRANCH_AT = '52{brnCode}'  -- parameter
                            and hd.shift_no IN ({shiftNo}) -- parameter
                            and dt.pay_types in ({payTypeIds}) -- parameter
                            and hd.com_code = dt.com_code
                            and hd.branch_code = dt.branch_code
                            and hd.pos_id = dt.pos_id
                            and hd.pos_day_id = dt.pos_day_id
                            and hd.shift_no = dt.shift_no
                            and hd.payment_id = dt.payment_id ) p
                            where a.com_code = p.com_code
                            and a.branch_code = p.branch_code
                            and a.pos_id = p.pos_id
                            and a.pos_day_id = p.pos_day_id
                            and a.shift_no = p.shift_no
                            and a.sale_id = p.sale_id )
                        ";
            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 60,
                CommandText = commandText
            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();

            var customerId = context.DopPosConfigs.FirstOrDefault(x => x.DocType == "Receive" && x.DocDesc == "Customer");

            int intSeqNo = 1;

            while (oracleDataReader.Read())
            {
                var pluNumber = oracleDataReader["PLU_NUMBER"].ToString();
                var masproduct = context.MasProducts.FirstOrDefault(x => x.PdId == pluNumber);

                POSReceive receive = new POSReceive
                {
                    Row = intSeqNo++,
                    CustCode = custCode == null ? "" : custCode.PdId,
                    JournalId = oracleDataReader[0].ToString() ?? string.Empty,
                    SiteId = oracleDataReader[1].ToString() ?? string.Empty,
                    BusinessDate = (DateTime)oracleDataReader[2],
                    ShiftNo = oracleDataReader[3].ToString() ?? string.Empty,
                    PluNumber = oracleDataReader[4].ToString() ?? string.Empty,
                    ItemName = oracleDataReader[5].ToString() ?? string.Empty,
                    SellQty = Convert.ToDecimal(oracleDataReader[6] ?? 0),
                    SellPrice = Convert.ToDecimal(oracleDataReader[7] ?? 0),
                    GoodsAmt = Convert.ToDecimal(oracleDataReader[8] ?? 0),
                    TaxAmt = Convert.ToDecimal(oracleDataReader[9] ?? 0),
                    DiscAmt = Convert.ToDecimal(oracleDataReader[10] ?? 0),
                    SumItemAmt = Convert.ToDecimal(oracleDataReader[11] ?? 0),
                    SubAmt = Convert.ToDecimal(oracleDataReader[12] ?? 0),
                    TotalAmt = Convert.ToDecimal(oracleDataReader[13] ?? 0),
                };

                posReceives.Add(receive);
            }

            con.Close();
            oracleDataReader.Close();
            return posReceives.OrderBy(x => x.JournalId).ToList();
        }

        //private string GetReceiveSqlString(DateTime fromDate, string brnCode, string payTypeIds, string shiftNo, string payTypePolice)
        //{
        //    return $@"
        //        select f4.journal_id, f4.site_id, f4.business_date, f4.shift_no,f5.plu_number, f5.item_name , f5.sell_qty , f5.sell_price, f5.goods_amt    , f5.tax_amt , f5.disc_amt                      
        //        , f5.goods_amt + f5.tax_amt + f5.disc_amt as sum_item_amt
        //        , f5.goods_amt + f5.tax_amt  as sub_amt
        //        , (f5.goods_amt *107/100) as total_amt
        //        from INF_POS_FUNCTION4(nolock) f4, INF_POS_FUNCTION5(nolock) f5
        //        where convert(varchar,f4.business_date,103) =  TO_DATE('{fromDate:dd/MM/yyyy}','dd/mm/yyyy')	-- parameter : doc_date
        //        and f4.site_id = '52{brnCode}'		 -- parameter : brn_code
        //        and f5.disc_group = 'N'
        //        and f4.journal_id = f5.journal_id
        //        and f4.shift_no = {shiftNo}			 -- parameter : period_no
        //        and exists ( select null
        //          from INF_POS_FUNCTION14(nolock) f14
        //          where f14.mop_code in ({payTypeIds}) -- paytypeid : cash
        //          and f14.journal_id = f4.journal_id
        //          and f14.site_id = f4.site_id )
        //        and not exists ( select null
        //          from INF_POS_FUNCTION14(nolock) f14
        //          where f14.mop_code in ({payTypePolice})  -- parameter : paytypeid => police
        //          and f14.journal_id = f4.journal_id
        //          and f14.site_id = f4.site_id )
        //        ";
        //}

        public async Task<PeriodCountResponse> GetPeriodCount(PeriodCountRequest request)
        {
            var docDate = request.FromDate.ToString("yyyy-MM-dd", new CultureInfo("en-US"));
            var response = new PeriodCountResponse();
            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 60,
                CommandText = $@"SELECT count(distinct  sm.SHIFT_NO) as ShiftNo
                                from {schema}shift_meter sm, raptorpos.day DAY
                                where  sm.com_code = day.com_code
                                  and sm.branch_code = day.branch_code
                                  and sm.shift_no = day.shift_no
                                  and sm.pos_id = day.pos_id
                                  and sm.pos_day_id = day.day_id
                                  and sm.branch_at = '52{request.BrnCode}'
                                  and trunc(day.account_date) = to_date('{docDate}','yyyy-mm-dd')"
            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();

            while (oracleDataReader.Read())
            {
                response.CountPeriod = Convert.ToInt32(oracleDataReader["ShiftNo"]);
            }

            oracleDataReader.Close();
            oracleDataReader.Dispose();
            con.Close();
            con.Dispose();

            return response;
        }

        protected int GetCashRunNumber(string comp_code, string brn_code, string pattern)
        {
            int runNumber = 0;

            var resp = context.SalCashsaleHds
                        .Where(x => x.CompCode == comp_code && x.BrnCode == brn_code && x.DocPattern == pattern)
                        .OrderByDescending(x => x.RunNumber)
                        .FirstOrDefault();

            if (resp != null)
            {
                runNumber = (resp.RunNumber ?? 0);
            }
            return runNumber;
        }


        protected int GetCreditRunNumber(string comp_code, string brn_code, string pattern)
        {
            int runNumber = 0;

            var resp = context.SalCreditsaleHds
                        .Where(x => x.CompCode == comp_code && x.BrnCode == brn_code && x.DocPattern == pattern)
                        .OrderByDescending(x => x.RunNumber)
                        .FirstOrDefault();

            if (resp != null)
            {
                runNumber = (resp.RunNumber ?? 0);
            }
            return runNumber;
        }

        protected int GetWithdrawRunNumber(string comp_code, string brn_code, string pattern)
        {
            int runNumber = 0;

            var resp = context.InvWithdrawHds
                        .Where(x => x.CompCode == comp_code && x.BrnCode == brn_code && x.DocPattern == pattern)
                        .OrderByDescending(x => x.RunNumber)
                        .FirstOrDefault();

            if (resp != null)
            {
                runNumber = (resp.RunNumber ?? 0);
            }
            return runNumber;
        }

        protected int GetReceiveRunNumber(string comp_code, string brn_code, string pattern)
        {
            int runNumber = 0;

            var resp = context.FinReceiveHds
                        .Where(x => x.CompCode == comp_code && x.BrnCode == brn_code && x.DocPattern == pattern)
                        .OrderByDescending(x => x.RunNumber)
                        .FirstOrDefault();

            if (resp != null)
            {
                runNumber = (resp.RunNumber ?? 0);
            }
            return runNumber;
        }
        protected string GenDocNo(DateTime systemDate, string BrnCode, string pattern, int runNumber, string docType)
        {
            string docno = "";
            var date = systemDate;
            var Brn = BrnCode;

            var patterns = (from hd in this.context.MasDocPatterns
                            join dt in this.context.MasDocPatternDts on hd.DocId equals dt.DocId
                            where hd.DocType == docType
                            select dt).ToList();

            docno = pattern;
            docno = docno.Replace("Pre", patterns.FirstOrDefault(x => x.DocCode == "[Pre]").DocValue);
            docno = docno.Replace("#", "") + runNumber.ToString("D" + patterns.FirstOrDefault(x => x.DocCode == "[#]").DocValue);
            return docno;
        }

        private async Task<Water> GetProfileByPass(string midNo, string DtStart, string dtEnd, string url)
        {
            var dict = new Dictionary<string, string>
            {
                { "MidNo", midNo },
                { "DtStart", DtStart },
                { "DtEnd", dtEnd }
            };

            var client = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(dict) };
            var httpResponse = await client.SendAsync(req);

            //var httpResponse = await _client.PostAsync(BaseUrl + "CrmPosGetProfileByPass", new JsonContent(new { MidNo = "395219101600448", DtStart = "20210301000000", DtEnd = "20210301235959" }));

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception("Cannot retrieve tasks");
            }

            var content = await httpResponse.Content.ReadAsStringAsync();
            var tasks = Newtonsoft.Json.JsonConvert.DeserializeObject<Water>(content);

            return tasks;
        }

        public async Task<int> GetPeriodWaterPOS(PeriodCountRequest request)
        {
            try
            {
                var resShift = new List<string>();
                var waterPosConfig = await context.DopPosConfigs.FirstOrDefaultAsync(x => x.DocType == "Withdraw" && x.DocDesc == "Water");
                var docDate = request.FromDate.ToString("yyyy-MM-dd", new CultureInfo("en-US"));

                OracleConnection con = new OracleConnection
                {
                    ConnectionString = _connectionString
                };
                con.Open();


                OracleCommand cmd = new OracleCommand
                {
                    Connection = con,
                    CommandTimeout = 60,
                    CommandText = $@"SELECT DISTINCT SHIFT_NO FROM {schema}shift_meter WHERE BRANCH_AT = '52{request.BrnCode}' AND TRUNC(BUSINESS_DATE_POS) = TO_DATE('{request.FromDate:dd/MM/yyyy}','dd/MM/yyyy') "

                };
                OracleDataReader oracleDataReader = cmd.ExecuteReader();
                var shiftNoList = new List<string>();
                while (oracleDataReader.Read())
                {
                    var shiftNo = oracleDataReader["SHIFT_NO"].ToString();
                    shiftNoList.Add(shiftNo);
                }

                if (shiftNoList.Any())
                {
                    foreach (var shiftNo in shiftNoList.OrderBy(x => x))
                    {
                        var pwater = waterPosConfig?.PdId;

                        cmd = new OracleCommand
                        {
                            Connection = con,
                            CommandTimeout = 60,
                            CommandText = $@"
                        select  tbl.site_id,tbl.business_date,tbl.shift_no,tbl.plu_number,sum(tbl.sell_qty) as sell_qty
                        from (
                        select  bh.bill_no journal_id, bh.branch_at AS site_id,trunc(bh.business_date)AS business_date ,bh.SHIFT_NO ,prod.product_code plu_number ,bd.sell_qty sell_qty
                        from raptorpos.bill_header bh, raptorpos.bill_detail bd, raptorpos.product prod, raptorpos.s_transaction_detail trd
                        where bh.com_code = bd.com_code
                            and bh.branch_code = bd.branch_code
                            and bh.pos_id = bd.pos_id
                            and bh.pos_day_id = bd.pos_day_id
                            and bh.shift_no = bd.shift_no
                            and bh.bill_id = bd.bill_id
                            and bd.ref_t_product_code = prod.product_code    
                            and bh.com_code = trd.com_code
                            and bh.branch_code = trd.branch_code
                            and bh.pos_id = trd.pos_id
                            and bh.pos_day_id = trd.pos_day_id
                            and bh.shift_no = trd.shift_no
                            and bh.sale_id = trd.sale_id
                            and bd.branch_code = trd.branch_code
                            and bd.pos_id = trd.pos_id
                            and bd.pos_day_id = trd.pos_day_id
                            and bd.shift_no = trd.shift_no
                            and bd.ref_t_trans_id = trd.ref_t_trans_id
                            and bd.item_id = trd.sale_item_id  
                            and trd.itemtype in ('O', 'N', 'F')
                            and bh.bill_status <> '0002'
                            and prod.product_type = 'F' 
                            and trunc(bh.BUSINESS_DATE) =  to_date('{docDate}','yyyy-mm-dd') -->parameter doc_date
                            AND bh.branch_at = '52{request.BrnCode}'		-->parameter brn_code
                            AND bh.shift_no = {shiftNo}	-->parameter period
                            and prod.PRODUCT_CODE  in ('{pwater}') --> parameter pd_id  
                        )tbl
                        group by tbl.site_id,tbl.business_date,tbl.shift_no,tbl.plu_number"
                        };

                        oracleDataReader = cmd.ExecuteReader();
                        if (oracleDataReader.HasRows)
                        {

                            resShift.Add(shiftNo);
                        }

                    }
                }
                oracleDataReader.Close();
                oracleDataReader.Dispose();
                con.Close();
                con.Dispose();

                return resShift.Count();
            }
            catch(Exception ex) 
            {
                return 0;
            }

        }

        public async Task<int> GetPeriodWaterMAX(PeriodCountRequest request)
        {
            try
            {
                var waterPosConfig = await context.DopPosConfigs.FirstOrDefaultAsync(x => x.DocType == "Withdraw" && x.DocDesc == "Water");
                var pwater = waterPosConfig?.PdId;
                var cnt = await (from hd in this.context.InvWithdrawHds
                                 join dt in this.context.InvWithdrawDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                                 where hd.DocStatus != "Cancel"
                                 && hd.CompCode == request.CompCode
                                 && hd.BrnCode == request.BrnCode
                                 && hd.LocCode == request.LocCode
                                 && hd.DocDate == request.FromDate
                                 && dt.PdId == pwater
                                 select new { hd }
                             ).ToListAsync();

                return cnt.Count();
            }
            catch(Exception ex)
            {
                return 0;
            }       
        }

        //private async Task<bool> IsDuplicateCashSale(SaveCashSaleResource pHeader , string pStrDocNo)
        //{
        //    var qryCashSale = context.SalCashsaleHds.Where(
        //        x => x.CompCode == pHeader.CompCode
        //        && x.BrnCode == pHeader.BrnCode
        //        && x.LocCode == pHeader.LocCode
        //        && x.DocNo == pStrDocNo
        //    );
        //    var result = await qryCashSale.AnyAsync();
        //    return result;
        //}
        //private async Task<bool> IsDuplicateWithDraw(SaveWithdrawResource pHeader, string pStrDocNo)
        //{
        //    var qryWithDraw = context.InvWithdrawHds.Where(
        //        x => x.CompCode == pHeader.CompCode
        //        && x.BrnCode == pHeader.BrnCode
        //        && x.LocCode == pHeader.LocCode
        //        && x.DocNo == pStrDocNo
        //    );
        //    var result = await qryWithDraw.AnyAsync();
        //    return result;
        //}
        //private async Task<bool> IsDuplicateCreditSale(SaveCreditSaleResource pHeader, string pStrDocNo)
        //{
        //    var qryCreditSale = context.InvWithdrawHds.Where(
        //        x => x.CompCode == pHeader.CompCode
        //        && x.BrnCode == pHeader.BrnCode
        //        && x.LocCode == pHeader.LocCode
        //        && x.DocNo == pStrDocNo
        //    );
        //    var result = await qryCreditSale.AnyAsync();
        //    return result;
        //}
    }
}
