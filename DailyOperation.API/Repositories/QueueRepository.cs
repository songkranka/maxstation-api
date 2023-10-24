using DailyOperation.API.Domain.Models;
using DailyOperation.API.Domain.Repositories;
using DailyOperation.API.Helpers;
using DailyOperation.API.Resources.POS;
using DailyOperation.API.Services;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DailyOperation.API.Repositories
{
    public class QueueRepository : SqlDataAccessHelper, IQueueRepository
    {
        private readonly string _connectionString;
        private readonly HttpClient _client;
        private const string schema = "raptorpos.";

        public QueueRepository(
            POSConnection posConnect,
            PTMaxstationContext context,
            HttpClient client
        ) : base(context)
        {
            _connectionString = posConnect.ConnectionString;
            _client = client;
        }

        public List<BranchSchedule> BranchScheduleList(BranchScheduleResource query)
        {
            var branchSchedules = new List<BranchSchedule>();
            var branchTranfer = context.SysConfigApis.FirstOrDefault(x => x.SystemId == "Queue" && x.ApiId == "M0001");

            if(branchTranfer != null)
            {
                string strCon = context.Database.GetConnectionString();
                string strSql = branchTranfer.ApiUrl;

                var sqlHelper = new SqlHelper(context);
                var branchPilots = sqlHelper.RawSqlQuery(strSql, x => new BranchPilot { BRN_CODE = (string)x[0], DOC_DATE = (DateTime)x[1] });

                foreach (var branchPilot in branchPilots)
                {
                    var branchSchedule = new BranchSchedule()
                    {
                        SITE_ID = branchPilot.BRN_CODE,
                        BUSINESS_DATE = branchPilot.DOC_DATE
                    };
                    branchSchedules.Add(branchSchedule);
                }
            }

            //var branchSchedules = new List<BranchSchedule>();
            //OracleConnection con = new OracleConnection
            //{
            //    ConnectionString = _connectionString
            //};
            //con.Open();



            //OracleCommand cmd = new OracleCommand
            //{
            //    Connection = con,
            //    CommandTimeout = 60,
            //    CommandText = $@"SELECT distinct SITE_ID, BUSINESS_DATE FROM {schema}FUNCTION2  WHERE TRUNC(FUNCTION2.BUSINESS_DATE) = TO_DATE('{query.FromDate:dd/MM/yyyy}','dd/MM/yyyy') ORDER BY TRUNC(BUSINESS_DATE) DESC"                
            //};

            //OracleDataReader oracleDataReader = cmd.ExecuteReader();

            //while (oracleDataReader.Read())
            //{
            //    var branchSchedule = new BranchSchedule
            //    {
            //        SITE_ID = oracleDataReader["SITE_ID"].ToString() ?? string.Empty,
            //        BUSINESS_DATE = Convert.ToDateTime(oracleDataReader["BUSINESS_DATE"])
            //    };

            //    branchSchedules.Add(branchSchedule);
            //}

            //oracleDataReader.Close();
            //oracleDataReader.Dispose();
            //con.Close();

            return branchSchedules;
        }

        public List<InfPosFunction2> GetPosFunction2(TranferPosResource query)
        {
            var PosFunction2List = new List<InfPosFunction2>();

            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };

            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 60,
                CommandText = $@"SELECT * FROM {schema}FUNCTION2  WHERE SITE_ID = '{query.BranchCode}' AND TRUNC(FUNCTION2.BUSINESS_DATE) = TO_DATE('{query.FromDate:dd/MM/yyyy}','dd/MM/yyyy') ORDER BY TRUNC(BUSINESS_DATE) DESC"
            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();

            while (oracleDataReader.Read())
            {
                var posFunction2 = new InfPosFunction2();
                posFunction2.SiteId = oracleDataReader["SITE_ID"].ToString() ?? string.Empty;
                posFunction2.BusinessDate = Convert.ToDateTime(oracleDataReader["BUSINESS_DATE"]);
                posFunction2.ShiftNo = oracleDataReader["SHIFT_NO"].ToString() ?? string.Empty;
                posFunction2.HoseId = Convert.ToInt32(oracleDataReader["HOSE_ID"] ?? 0);
                posFunction2.PosId = Convert.ToInt32(oracleDataReader["POS_ID"] ?? 0);
                posFunction2.PosName = oracleDataReader["POS_NAME"].ToString() ?? string.Empty;
                posFunction2.ShiftId = Convert.ToInt32(oracleDataReader["SHIFT_ID"] ?? 0);
                posFunction2.ShiftDesc = oracleDataReader["SHIFT_DESC"].ToString() ?? string.Empty;
                posFunction2.PumpId = Convert.ToInt32(oracleDataReader["PUMP_ID"] ?? 0);
                posFunction2.GradeId = oracleDataReader["GRADE_ID"].ToString() ?? string.Empty;
                posFunction2.GradeName = oracleDataReader["GRADE_NAME"].ToString() ?? string.Empty;
                posFunction2.GradeName2 = oracleDataReader["GRADE_NAME2"].ToString() ?? string.Empty;
                posFunction2.OpenMeterValue = Convert.ToDecimal(oracleDataReader["OPEN_METER_VALUE"] ?? 0);
                posFunction2.CloseMeterValue = Convert.ToDecimal(oracleDataReader["CLOSE_METER_VALUE"] ?? 0);
                posFunction2.TotalValue = Convert.ToDecimal(oracleDataReader["TOTAL_VALUE"] ?? 0);
                posFunction2.OpenMeterVolume = Convert.ToDecimal(oracleDataReader["OPEN_METER_VOLUME"] ?? 0);
                posFunction2.CloseMeterVolume = Convert.ToDecimal(oracleDataReader["CLOSE_METER_VOLUME"] ?? 0);
                posFunction2.TotalVolume = Convert.ToDecimal(oracleDataReader["TOTAL_VOLUME"] ?? 0);
                posFunction2.CreatedDate = DateTime.Now;
                posFunction2.CreatedBy = "System";
                

                PosFunction2List.Add(posFunction2);
            }

            oracleDataReader.Close();
            oracleDataReader.Dispose();
            con.Close();

            return PosFunction2List;
        }

        public List<InfPosFunction4> GetPosFunction4(TranferPosResource query)
        {
            var posFunction4List = new List<InfPosFunction4>();

            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };

            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 60,
                CommandText = $@"SELECT * FROM {schema}FUNCTION4  WHERE SITE_ID = '{query.BranchCode}' AND TRUNC(FUNCTION4.BUSINESS_DATE) = TO_DATE('{query.FromDate:dd/MM/yyyy}','dd/MM/yyyy') ORDER BY TRUNC(BUSINESS_DATE) DESC"
            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();

            while (oracleDataReader.Read())
            {
                var posFunction4 = new InfPosFunction4();
                posFunction4.SiteId = oracleDataReader["SITE_ID"].ToString() ?? string.Empty;
                posFunction4.BusinessDate = Convert.ToDateTime(oracleDataReader["BUSINESS_DATE"]);
                posFunction4.ShiftNo = oracleDataReader["SHIFT_NO"].ToString() ?? string.Empty;
                posFunction4.JournalId = oracleDataReader["JOURNAL_ID"].ToString() ?? string.Empty;
                posFunction4.JournalDate = Convert.ToDateTime(oracleDataReader["JOURNAL_DATE"]);
                posFunction4.JournalStatus = oracleDataReader["JOURNAL_STATUS"].ToString() ?? string.Empty;
                posFunction4.PosId = Convert.ToInt32(oracleDataReader["POS_ID"] ?? 0);
                posFunction4.UserId = oracleDataReader["USER_ID"].ToString() ?? string.Empty;
                posFunction4.Username = oracleDataReader["USERNAME"].ToString() ?? string.Empty;
                posFunction4.ShiftId = Convert.ToInt32(oracleDataReader["SHIFT_ID"] ?? 0);
                posFunction4.TransNo = Convert.ToInt32(oracleDataReader["TRANS_NO"] ?? 0);
                posFunction4.Billno = oracleDataReader["BILLNO"].ToString() ?? string.Empty;
                posFunction4.Taxinvno = oracleDataReader["TAXINVNO"].ToString() ?? string.Empty;
                posFunction4.MaxCardNumber = oracleDataReader["MAX_CARD_NUMBER"].ToString() ?? string.Empty;
                posFunction4.TotalGoodsamt = Convert.ToDecimal(oracleDataReader["TOTAL_GOODSAMT"] ?? 0);
                posFunction4.TotalDiscamt = Convert.ToDecimal(oracleDataReader["TOTAL_DISCAMT"] ?? 0);
                posFunction4.TotalTaxamt = Convert.ToDecimal(oracleDataReader["TOTAL_TAXAMT"] ?? 0);
                posFunction4.TotalPaidAmt = Convert.ToDecimal(oracleDataReader["TOTAL_PAID_AMT"] ?? 0);
                posFunction4.CustomerId = oracleDataReader["CUSTOMER_ID"].ToString() ?? string.Empty;
                posFunction4.LicNo = oracleDataReader["LIC_NO"].ToString() ?? string.Empty;
                posFunction4.UserCardNo = oracleDataReader["USER_CARD_NO"].ToString() ?? string.Empty;
                posFunction4.Miles = oracleDataReader["MILES"].ToString() ?? string.Empty;
                posFunction4.CreatedDate = DateTime.Now;
                posFunction4.CreatedBy = "System";
                posFunction4List.Add(posFunction4);
            }

            oracleDataReader.Close();
            oracleDataReader.Dispose();
            con.Close();

            return posFunction4List;
        }

        public List<InfPosFunction5> GetPosFunction5(TranferPosResource query)
        {
            var posFunction5List = new List<InfPosFunction5>();

            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };

            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 60,
                CommandText = $@"SELECT f5.* FROM {schema}FUNCTION5 f5
                                 INNER JOIN {schema}function4 f4 on f5.JOURNAL_ID = f4.JOURNAL_ID
                                 WHERE f4.SITE_ID = '{query.BranchCode}' AND TRUNC(f4.BUSINESS_DATE) = TO_DATE('{query.FromDate:dd/MM/yyyy}','dd/MM/yyyy') ORDER BY TRUNC(BUSINESS_DATE) DESC"
            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();

            while (oracleDataReader.Read())
            {
                var posFunction5 = new InfPosFunction5();
                posFunction5.JournalId = oracleDataReader["JOURNAL_ID"].ToString() ?? string.Empty;
                posFunction5.Runno = Convert.ToInt32(oracleDataReader["RUNNO"] ?? 0);
                posFunction5.ItemType = oracleDataReader["ITEM_TYPE"].ToString() ?? string.Empty;
                posFunction5.ItemCode = oracleDataReader["ITEM_CODE"].ToString() ?? string.Empty;
                posFunction5.ItemName = oracleDataReader["ITEM_NAME"].ToString() ?? string.Empty;
                posFunction5.PluNumber = oracleDataReader["PLU_NUMBER"].ToString() ?? string.Empty;
                posFunction5.DiscGroup = oracleDataReader["DISC_GROUP"].ToString() ?? string.Empty;
                posFunction5.ProductCodesap = oracleDataReader["PRODUCT_CODESAP"].ToString() ?? string.Empty;
                posFunction5.SellQty = Convert.ToDecimal(oracleDataReader["SELL_QTY"] ?? 0);
                posFunction5.SellPrice = Convert.ToDecimal(oracleDataReader["SELL_PRICE"] ?? 0);
                posFunction5.GoodsAmt = Convert.ToDecimal(oracleDataReader["GOODS_AMT"] ?? 0);
                posFunction5.TaxRate = oracleDataReader["TAX_RATE"].ToString() ?? string.Empty;
                posFunction5.TaxAmt = Convert.ToDecimal(oracleDataReader["TAX_AMT"] ?? 0);
                posFunction5.HoseId = Convert.ToInt32(oracleDataReader["HOSE_ID"] ?? 0);
                posFunction5.PumpId = Convert.ToInt32(oracleDataReader["PUMP_ID"] ?? 0);
                posFunction5.DeliveryId = Convert.ToInt32(oracleDataReader["DELIVERY_ID"] ?? 0);
                posFunction5.TankId = Convert.ToInt32(oracleDataReader["TANK_ID"] ?? 0);
                posFunction5.DeliveryType = Convert.ToInt32(oracleDataReader["DELIVERY_TYPE"] ?? 0);
                posFunction5.DiscAmt = Convert.ToDecimal(oracleDataReader["DISC_AMT"] ?? 0);
                posFunction5.ShiftId = Convert.ToInt32(oracleDataReader["SHIFT_ID"] ?? 0);
                posFunction5.PosId = Convert.ToInt32(oracleDataReader["POS_ID"] ?? 0);
                posFunction5.CreatedDate = DateTime.Now;
                posFunction5.CreatedBy = "System";
                posFunction5List.Add(posFunction5);
            }

            oracleDataReader.Close();
            oracleDataReader.Dispose();
            con.Close();

            return posFunction5List;
        }

        public List<InfPosFunction14> GetPosFunction14(TranferPosResource query)
        {
            var PosFunction14List = new List<InfPosFunction14>();

            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };

            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 60,
                CommandText = $@"SELECT * FROM {schema}FUNCTION14  WHERE SITE_ID = '{query.BranchCode}' AND TRUNC(FUNCTION14.BUSINESS_DATE) = TO_DATE('{query.FromDate:dd/MM/yyyy}','dd/MM/yyyy') ORDER BY TRUNC(BUSINESS_DATE) DESC"
            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();

            while (oracleDataReader.Read())
            {
                var posFunction14 = new InfPosFunction14();
                posFunction14.SiteId = oracleDataReader["SITE_ID"].ToString() ?? string.Empty;
                posFunction14.BusinessDate = Convert.ToDateTime(oracleDataReader["BUSINESS_DATE"]);
                posFunction14.ShiftNo = oracleDataReader["SHIFT_NO"].ToString() ?? string.Empty;
                posFunction14.JournalId = oracleDataReader["JOURNAL_ID"].ToString() ?? string.Empty;
                posFunction14.MopCode = oracleDataReader["MOP_CODE"].ToString() ?? string.Empty;
                posFunction14.PosId = Convert.ToInt32(oracleDataReader["POS_ID"] ?? 0);
                posFunction14.PosName = oracleDataReader["POS_NAME"].ToString() ?? string.Empty;
                posFunction14.ShiftId = Convert.ToInt32(oracleDataReader["SHIFT_ID"] ?? 0);
                posFunction14.ShiftDesc = oracleDataReader["SHIFT_DESC"].ToString() ?? string.Empty;
                posFunction14.MopInfo = oracleDataReader["MOP_INFO"].ToString() ?? string.Empty;
                posFunction14.Amount = Convert.ToDecimal(oracleDataReader["AMOUNT"] ?? 0);
                posFunction14.Bstatus = oracleDataReader["BSTATUS"].ToString() ?? string.Empty;
                posFunction14.InsertTimestamp = oracleDataReader["INSERT_TIMESTAMP"].ToString() ?? string.Empty;
                posFunction14.Pono = oracleDataReader["PONO"].ToString() ?? string.Empty;
                posFunction14.CardNo = oracleDataReader["CARD_NO"].ToString() ?? string.Empty;
                posFunction14.BranchAt = oracleDataReader["BRANCH_AT"].ToString() ?? string.Empty;
                posFunction14.CreatedDate = DateTime.Now;
                posFunction14.CreatedBy = "System";
                PosFunction14List.Add(posFunction14);
            }

            oracleDataReader.Close();
            oracleDataReader.Dispose();
            con.Close();

            return PosFunction14List;
        }

        public List<InfPosFunction2> CheckDuplicateFunction2(TranferPosResource query, List<InfPosFunction2> posFunction2List)
        {
            var function2s = context.InfPosFunction2s.Where(x => x.SiteId == query.BranchCode && x.BusinessDate.Year == query.FromDate.Year && x.BusinessDate.Month == query.FromDate.Month && x.BusinessDate.Day == query.FromDate.Day).ToList();
            posFunction2List = posFunction2List.Where(x => !function2s.Any(y => y.SiteId == x.SiteId && y.BusinessDate.Day == x.BusinessDate.Day && y.BusinessDate.Month == x.BusinessDate.Month && y.BusinessDate.Day == x.BusinessDate.Day && y.ShiftNo == x.ShiftNo && y.HoseId == x.HoseId)).ToList();

            return posFunction2List;
        }

        public List<InfPosFunction4> CheckDuplicateFunction4(TranferPosResource query, List<InfPosFunction4> posFunction4List)
        {
            var function4s = context.InfPosFunction4s.Where(x => x.SiteId == query.BranchCode && x.BusinessDate.Year == query.FromDate.Year && x.BusinessDate.Month == query.FromDate.Month && x.BusinessDate.Day == query.FromDate.Day).ToList();
            posFunction4List = posFunction4List.Where(x => !function4s.Any(y => y.SiteId == x.SiteId && y.BusinessDate.Day == x.BusinessDate.Day && y.BusinessDate.Month == x.BusinessDate.Month && y.BusinessDate.Day == x.BusinessDate.Day && y.ShiftNo == x.ShiftNo && y.JournalId == x.JournalId)).ToList();

            return posFunction4List;
        }

        public List<InfPosFunction5> CheckDuplicateFunction5(TranferPosResource query, List<InfPosFunction5> posFunction5List)
        {
            var function5s = (from f5 in context.InfPosFunction5s
                              join f4 in context.InfPosFunction4s on f5.JournalId equals f4.JournalId
                              where f4.SiteId == query.BranchCode && f4.BusinessDate.Year == query.FromDate.Year && f4.BusinessDate.Month == query.FromDate.Month && f4.BusinessDate.Day == query.FromDate.Day
                              select f5).ToList();
            posFunction5List = posFunction5List.Where(x => !function5s.Any(y => y.JournalId == x.JournalId && y.Runno == x.Runno)).ToList();
            return posFunction5List;
        }
        public List<InfPosFunction14> CheckDuplicateFunction14(TranferPosResource query, List<InfPosFunction14> posFunction14List)
        {
            var function14s = context.InfPosFunction14s.Where(x => x.SiteId == query.BranchCode && x.BusinessDate.Year == query.FromDate.Year && x.BusinessDate.Month == query.FromDate.Month && x.BusinessDate.Day == query.FromDate.Day).ToList();
            posFunction14List = posFunction14List.Where(x => !function14s.Any(y => y.SiteId == x.SiteId && y.BusinessDate.Day == x.BusinessDate.Day && y.BusinessDate.Month == x.BusinessDate.Month && y.BusinessDate.Day == x.BusinessDate.Day && y.ShiftNo == x.ShiftNo && y.JournalId == x.JournalId)).ToList();

            return posFunction14List;
        }

        public async Task AddFunction2Async(List<InfPosFunction2> posFunction2List)
        {
            await context.InfPosFunction2s.AddRangeAsync(posFunction2List);
        }

        public async Task AddFunction4Async(List<InfPosFunction4> posFunction4List)
        {
            await context.InfPosFunction4s.AddRangeAsync(posFunction4List);
        }

        public async Task AddFunction5Async(List<InfPosFunction5> posFunction5List)
        {
            await context.InfPosFunction5s.AddRangeAsync(posFunction5List);
        }

        public async Task AddFunction14Async(List<InfPosFunction14> posFunction14List)
        {
            await context.InfPosFunction14s.AddRangeAsync(posFunction14List);
        }
    }
}
