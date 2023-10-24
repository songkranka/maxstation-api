using AutoMapper;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using PostDay.API.Domain.Models.Queries;
using PostDay.API.Domain.Repositories;
using PostDay.API.Helpers;
using PostDay.API.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Repositories
{
    public class LoadTestRepository : SqlDataAccessHelper, ILoadTestRepository
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public LoadTestRepository(
            PTMaxstationContext context,
            IMapper mapper,
            IUnitOfWork unitOfWork) : base(context)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }



        public async Task<QueryResult<SalCashsaleHd>> ListAsync(CashSaleHdQuery query)
        {


            if (query == null)
            {
                return null;
            }
            string strIsoLevel = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";
            string strSelect = @"select doc_no , doc_date , net_amt , doc_status , guid from SAL_CASHSALE_HD(nolock)";
            string strCount = @"select COUNT(*) from SAL_CASHSALE_HD(nolock)";
            string strWhere = @" where 1=1 ";
            string strOrderBy = @" order by CREATED_DATE desc ";
            string strComCode = DefaultService.EncodeSqlString(query.CompCode);
            if (!0.Equals(strComCode.Length))
            {
                strWhere += $" and COMP_CODE = '{strComCode}'";
            }
            string strBrnCode = DefaultService.EncodeSqlString(query.BrnCode);
            if (!0.Equals(strBrnCode.Length))
            {
                strWhere += $" and BRN_CODE = '{strBrnCode}'";
            }
            if (query.FromDate.HasValue && query.ToDate.HasValue)
            {
                strWhere += $" and DOC_DATE between '{query.FromDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}' and '{query.ToDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}'";
            }
            string strKeyWord = DefaultService.EncodeSqlString(query.Keyword);
            if (!0.Equals(strKeyWord.Length))
            {
                strWhere += $" and ( DOC_NO like '%{strKeyWord}%' or DOC_STATUS like '%{strKeyWord}%' )";
            }
            string strCon = context.Database.GetConnectionString();
            int intTotal = await DefaultService.ExecuteScalar<int>(strCon, strIsoLevel + strCount + strWhere);


            string strPage = string.Empty;
            if (query.Page > 0 && query.ItemsPerPage > 0)
            {
                strPage = $" OFFSET {(query.Page - 1) * query.ItemsPerPage} row fetch next {query.ItemsPerPage} row only";
            }
            var listCashSale = await DefaultService.GetEntityFromSql<List<SalCashsaleHd>>(
                strCon, strIsoLevel + strSelect + strWhere + strOrderBy + strPage
            );
                        

            return new QueryResult<SalCashsaleHd>
            {
                Items = listCashSale ?? new List<SalCashsaleHd>(),
                TotalItems = intTotal,
                ItemsPerPage = query.ItemsPerPage
            };
        }


    }
}
