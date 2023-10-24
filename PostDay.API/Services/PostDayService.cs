using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PostDay.API.Domain.Models.PostDay;
using PostDay.API.Domain.Models.Request;
using PostDay.API.Domain.Models.RestAPI;
using PostDay.API.Domain.Repositories;
using PostDay.API.Domain.Services;
using PostDay.API.Domain.Services.Communication.PostDay;
using PostDay.API.Resources.PostDay;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace PostDay.API.Services
{
    public class PostDayService : IPostDayService
    {

        private readonly IPostDayRepository _postdayRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private PTMaxstationContext Context;

        public PostDayService(IPostDayRepository postdayRepository,IUnitOfWork unitOfWork,PTMaxstationContext context,IServiceScopeFactory serviceScopeFactory)
        {
            _postdayRepository = postdayRepository;
            _unitOfWork = unitOfWork;
            Context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public Task<PostDayResponse> GetDocument(GetDocumentRequest req)
        {
            return _postdayRepository.GetDocument(req);
        }

        //public Task<SaveDocumentResponse> SaveDocument(SaveDocumentRequest req)
        //{
        //    return _postdayRepository.SaveDocument(req);
        //}
        public async Task<SaveDocumentResponse> SaveDocument(SaveDocumentRequest req)
        {
            var result = await _postdayRepository.SaveDocument(req);
            //if("Success".Equals( result.Status))
            //{
            //    var docDate = DateTime.ParseExact(req.DopPostdayHd.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //    var addStockParam = new AddStockParam()
            //    {
            //        BrnCode = req.DopPostdayHd.BrnCode,
            //        CompCode = req.DopPostdayHd.CompCode,
            //        CreatedBy = req.DopPostdayHd.User,
            //        LocCode = req.DopPostdayHd.LocCode,
            //        SysDate = docDate
            //    };
            //    await AddStock(addStockParam);
            //    var lastDayOfMonth = DateTime.DaysInMonth(docDate.Year, docDate.Month);
            //    if (docDate.Day == lastDayOfMonth)
            //    {
            //        var addStockMonthlyParam = new AddStockMonthlyParam()
            //        {
            //            BrnCode = req.DopPostdayHd.BrnCode,
            //            CompCode = req.DopPostdayHd.CompCode,
            //            CreatedBy = req.DopPostdayHd.User,
            //            LocCode = req.DopPostdayHd.LocCode,
            //            Month = docDate.Month,
            //            Year = docDate.Year
            //        };
            //        await AddStockMonthly(addStockMonthlyParam);
            //    }
            //}
            return result;
            //return _postdayRepository.SaveDocument(req);
        }
        public async Task<int> AddStock(AddStockParam param)
        {
            if(param == null)
            {
                return 0;
            }
			string strFilePath = @"Resources/SqlScript/SP_ADD_STOCK.sql";
			string strSql = await File.ReadAllTextAsync(strFilePath);
            strSql = string.Format(strSql, 
                DefaultService.EncodeSqlString( param.CompCode),
                DefaultService.EncodeSqlString(param.BrnCode),
                DefaultService.EncodeSqlString(param.LocCode),
                DefaultService.EncodeSqlString(param.CreatedBy), 
                param.SysDate.ToString("yyyy-MM-dd"));
            string strCon = Context.Database.GetConnectionString();
            int result = await DefaultService.ExecuteNonQuery(strCon, strSql);
            return result;
        }
        public async Task<int> AddStockMonthly(AddStockMonthlyParam param)
        {
            if (param == null)
            {
                return 0;
            }
            string strFilePath = @"Resources/SqlScript/SP_ADD_STOCK_MONTHLY.sql";
			string strSql = await File.ReadAllTextAsync(strFilePath);
            strSql = string.Format(strSql,
                DefaultService.EncodeSqlString(param.CompCode),
                DefaultService.EncodeSqlString(param.BrnCode),
                DefaultService.EncodeSqlString(param.LocCode),
                DefaultService.EncodeSqlString(param.CreatedBy),
                param.Year , param.Month);
            string strCon = Context.Database.GetConnectionString();
            int result = await DefaultService.ExecuteNonQuery(strCon, strSql);
            return result;
        }
		public async Task<DateTime?> TestSelectDate2()
        {
            string strFilePath = @"Resources/SqlScript/Test.sql";
            if (!File.Exists(strFilePath))
            {
				return null;
            }
            string strSql =await File.ReadAllTextAsync(strFilePath);
			string strCon = Context.Database.GetConnectionString();
			var apiResult = await DefaultService.ExecuteScalar<DateTimeOffset>(strCon, strSql);
			if(apiResult == null)
            {
				return null;
            }
			var result = apiResult.UtcDateTime;
			return result;
		}


		public async Task<DateTime> TestSelectDate ()
        {
            //string strFilePath = @"Resources/SqlScript/Test.sql";
            string strSql = @"select getdate()";            
            string strCon = Context.Database.GetConnectionString();
            DateTime result = await DefaultService.ExecuteScalar<DateTime>(strCon, strSql);
            return result;
        }

        public async Task<DataTable> GetDopValidData(GetDopValidDataParam param)
        {
            return await _postdayRepository.GetDopValidData(param);
        }

        public async Task<SaveDocumentResponse> CreateTaxInvoice(PostDayResource req)
        {
            SaveDocumentResponse response = new SaveDocumentResponse();

            await _postdayRepository.UpdateCreditSaleAsync(req);
            await _postdayRepository.CreateTaxInvoiceAsync(req);
            await _unitOfWork.CompleteAsync();

            response.StatusCode = 200;
            response.Message = "CreateTaxInvoice Complete";
            return response;
        }
    }
}
