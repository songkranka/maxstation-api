using Abp.Linq.Expressions;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MasterData.API.Resources.Customer;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private PTMaxstationContext _context = null;
        private IUnitOfWork _unitOfWorf = null;
        public CustomerService(
            ICustomerRepository customerRepository , PTMaxstationContext pContext , IUnitOfWork pUnitOfWork)
        {
            _customerRepository = customerRepository;
            _context = pContext;
            _unitOfWorf = pUnitOfWork;
        }

        public List<MasCustomer> GetCustomerList(CustomerRequest req)
        {
            List<MasCustomer> customerList = new List<MasCustomer>();
            customerList = _customerRepository.GetCustomerList(req);
            return customerList;
        }

        public async Task<Revenue> GetTaxInfoAsync(TaxQueryResource query)
        {
            RevenueResponse data = new RevenueResponse();
            string url = $"http://taxs.pt.co.th/pttax-ws/customer-profile-api/getCustomerProfile?tin={query.tin}&branch={query.branch}";
            using (var client = new HttpClient())
            {
                //HTTP POST
                HttpContent content = new StringContent("application/json");
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<RevenueResponse>(result);
                if(data.dataList == null)
                {
                    throw new Exception(data.statusMessage);
                }
                Revenue result2 = data.dataList.FirstOrDefault();
                MasCustomer cust = null;
                cust = await _customerRepository.GetCustomerFromCitizenId(query.tin);
                result2.custCode = (cust?.CustCode ?? string.Empty).Trim();
                return result2;
            }
        }

        public async Task<QueryResult<MasCustomer>> ListAsync(CustomerQuery query)
        {

            return await _customerRepository.ListAsync(query);
        }

        public async Task<ModelGetCustomerListResult> GetCustomerList2(ModelGetCustomerLisParam param)
        {
            if(param == null)
            {
                return null;
            }
            var result = new ModelGetCustomerListResult();
            string strCompCode = (param.CompCode ?? string.Empty).Trim();            
            string strKeyWord = (param.KeyWord ?? string.Empty).Trim();
            var esCustomer = PredicateBuilder.New<MasCustomer>(x=> true);
            
            if(strCompCode.Length > 0)
            {
                esCustomer = esCustomer.And(x => x.CompCode == strCompCode);
            }
            if(strKeyWord.Length > 0)
            {
                var esCustomer2 = PredicateBuilder.New<MasCustomer>(x => false);
                esCustomer2 = esCustomer2.Or(x => !string.IsNullOrEmpty(x.CustCode)
                   && (x.CustCode.Contains(strKeyWord) 
                   //|| strKeyWord.Contains(x.CustCode)
                ));

                esCustomer2 = esCustomer2.Or(x => !string.IsNullOrEmpty(x.CustName)
                   && (x.CustName.Contains(strKeyWord) 
                   //|| strKeyWord.Contains(x.CustName)
                ));

                esCustomer2 = esCustomer2.Or(x => !string.IsNullOrEmpty(x.Phone)
                   && (x.Phone.Contains(strKeyWord) 
                   //|| strKeyWord.Contains(x.Phone)
                ));

                esCustomer = esCustomer.And(esCustomer2);

            }
            var qryCustomer = _context.MasCustomers.Where(esCustomer).AsNoTracking();
            result.TotalItem = await qryCustomer.CountAsync();
            if(param.ItemPerPage > 0 && param.PageIndex > 0)
            {
                qryCustomer = qryCustomer
                    .Skip((param.PageIndex - 1) * param.ItemPerPage)
                    .Take(param.ItemPerPage);
            }
            result.ArrCustomer = await qryCustomer.ToArrayAsync();
            return result;
        }

        public async Task<ModelCustomer> GetCustomer(string pStrGuid)
        {
            pStrGuid = (pStrGuid ?? string.Empty).Trim();
            if(pStrGuid.Length == 0)
            {
                return null;
            }
            
            var gid = Guid.Empty;
            if(!Guid.TryParse(pStrGuid , out gid))
            {
                return null;
            }
            var result = new ModelCustomer();
            var qryCustomer = _context.MasCustomers.AsNoTracking()
                .Where(x =>x.Guid == gid);
            result.Customer = await qryCustomer.FirstOrDefaultAsync();

            if(result.Customer != null)
            {
                string strCusCode = string.Empty;
                strCusCode = (result.Customer?.CustCode ?? string.Empty).Trim();
                if(strCusCode.Length > 0)
                {
                    var qryCusCar = _context.MasCustomerCars.AsNoTracking()
                        .Where(x => x.CustCode == strCusCode);
                    result.ArrCustomerCar = await qryCusCar.ToArrayAsync();
                }

            }


            return result;
        }

        public async Task<bool> CheckDuplicateCustCode(string pStrCuscode)
        {
            pStrCuscode = (pStrCuscode ?? string.Empty).Trim();
            if(pStrCuscode.Length == 0)
            {
                return false;
            }
            bool result = await _context.MasCustomers.AnyAsync(x => x.CustCode == pStrCuscode);
            return result;
        }

        public async Task<ModelCustomer> InsertCustomer(ModelCustomer param)
        {
            if(param == null)
            {
                return null;
            }
            var cus = param.Customer;
            if(cus != null)
            {
                cus.Guid = Guid.NewGuid();
                cus.CreatedDate = DateTime.Now;
                await _context.AddAsync(cus);
                var qryCusCar = _context.MasCustomerCars
                    .Where(x => x.CustCode == cus.CustCode)
                    .AsNoTracking();
                _context.MasCustomerCars.RemoveRange(qryCusCar);
                var arrCar = param.ArrCustomerCar;
                if (arrCar != null && arrCar.Length > 0)
                {
                    arrCar = arrCar.Where(x => x != null).ToArray();
                }
                if (arrCar != null && arrCar.Length > 0)
                {
                    foreach (var car in arrCar)
                    {
                        car.CreatedDate = cus.CreatedDate;
                        car.CreatedBy = cus.CreatedBy;
                    }
                    await _context.MasCustomerCars.AddRangeAsync(arrCar);
                    param.ArrCustomerCar = arrCar;
                }
                await _unitOfWorf.CompleteAsync();

            }
            return param;
        }

        public async Task<ModelCustomer> UpdateCustomer(ModelCustomer param)
        {
            if (param == null)
            {
                return null;
            }
            var cus = param.Customer;
            if (cus != null)
            {
                cus.UpdatedDate = DateTime.Now;
                var entCus = _context.Attach(cus);
                entCus.State = EntityState.Modified;
                entCus.Property(x => x.CompCode).IsModified = false;
                entCus.Property(x => x.CreatedBy).IsModified = false;
                entCus.Property(x => x.CreatedDate).IsModified = false;
                entCus.Property(x => x.CustCode).IsModified = false;
                entCus.Property(x => x.Guid).IsModified = false;
                entCus.Property(x => x.BrnCode).IsModified = false;

                var qryCusCar = _context.MasCustomerCars
                    .Where(x => x.CustCode == cus.CustCode)
                    .AsNoTracking();
                _context.MasCustomerCars.RemoveRange(qryCusCar);
                var arrCar = param.ArrCustomerCar;
                if (arrCar != null && arrCar.Length > 0)
                {
                    arrCar = arrCar.Where(x => x != null).ToArray();
                }
                if (arrCar != null && arrCar.Length > 0)
                {
                    foreach (var car in arrCar)
                    {
                        car.CreatedDate = cus.CreatedDate;
                        car.CreatedBy = cus.CreatedBy;
                        car.UpdatedDate = cus.UpdatedDate;
                        car.UpdatedBy = cus.UpdatedBy;
                    }
                    await _context.MasCustomerCars.AddRangeAsync(arrCar);
                    param.ArrCustomerCar = arrCar;
                }
                await _unitOfWorf.CompleteAsync();
            }
            return param;
        }

        public async Task<MasCustomer> UpdateStatus(MasCustomer param)
        {
            if(param == null)
            {
                return null;
            }
            var entCus = _context.Attach(param);
            param.UpdatedDate = DateTime.Now;
            entCus.Property(x => x.UpdatedBy).IsModified = true;
            entCus.Property(x => x.CustStatus).IsModified = true;
            await _unitOfWorf.CompleteAsync();
            return param;
        }

        public async Task<QueryResult<MasCustomer>> FindAllAsync(CustomerQuery query)
        {
            return await _customerRepository.FindAllAsync(query);
        }
    }
}
