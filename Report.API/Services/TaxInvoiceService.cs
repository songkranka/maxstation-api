using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Services
{

    public class TaxInvoiceService : ITaxInvoiceService
    {
        private readonly ITaxInvoiceRepository _taxInvoiceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TaxInvoiceService(
            ITaxInvoiceRepository taxInvoiceRepository,
            IUnitOfWork unitOfWork)
        {
            _taxInvoiceRepository = taxInvoiceRepository;
            _unitOfWork = unitOfWork;
        }

        //public async Task<TaxInvoiceResponse> GetReportData(string guid,  string printby)
        //{
        //    try
        //    {
        //        var maxPrint = 999;
        //        var salTaxInvoice = await _taxInvoiceRepository.FindByIdAsync(guid, printby);

        //        if (salTaxInvoice.SalTaxinvoiceHd == null)
        //            return new TaxInvoiceResponse("TaxInvoice not found.");

        //        if(salTaxInvoice.SalTaxinvoiceHd.PrintCount >= maxPrint)
        //            return new TaxInvoiceResponse("Print max limit");

        //        salTaxInvoice.SalTaxinvoiceHd.PrintCount = salTaxInvoice.SalTaxinvoiceHd.PrintCount == null ? salTaxInvoice.SalTaxinvoiceHd.PrintCount = 1 : salTaxInvoice.SalTaxinvoiceHd.PrintCount + 1;
        //        salTaxInvoice.SalTaxinvoiceHd.PrintBy = printby;
        //        salTaxInvoice.SalTaxinvoiceHd.PrintDate = DateTime.Now;

        //        _taxInvoiceRepository.UpdateSalTaxInvoiceAsync(salTaxInvoice.SalTaxinvoiceHd);
        //        await _unitOfWork.CompleteAsync();


        //        return new TaxInvoiceResponse(salTaxInvoice);
        //    }
        //    catch(Exception ex)
        //    {
        //        string strMessage = ex.StackTrace;
        //        while (ex.InnerException != null) ex = ex.InnerException;
        //        strMessage = ex.Message + Environment.NewLine + strMessage;
        //        // Do some logging stuff
        //        return new TaxInvoiceResponse($"An error occurred when saving the taxinvoice: {strMessage}");
        //    }
            
        //}

        //public async Task<TaxInvoiceListResponse> GetTaxInvoiceListAsync(string guid, string printby)
        //{
        //    try
        //    {
        //        //var maxPrint = 999;
        //        var salTaxInvoice = await _taxInvoiceRepository.GetTaxInvoiceListAsync(guid, printby);

        //        if (salTaxInvoice.Count == 0) 
        //            return new TaxInvoiceListResponse("TaxInvoice not found.");

        //        return new TaxInvoiceListResponse(salTaxInvoice);
        //    }
        //    catch (Exception ex)
        //    {
        //        string strMessage = ex.StackTrace;
        //        while (ex.InnerException != null) ex = ex.InnerException;
        //        strMessage = ex.Message + Environment.NewLine + strMessage;
        //        // Do some logging stuff
        //        return new TaxInvoiceListResponse($"An error occurred when saving the taxinvoice: {strMessage}");
        //    }

        //}

        public async Task<TaxInvoiceHd> GetTaxInvoice(string guid, string empcode)
        {
            try
            {
                //var maxPrint = 999;
                var salTaxInvoice = await _taxInvoiceRepository.GetTaxInvoiceAsync(guid, empcode);

                if (salTaxInvoice == null)
                    throw new Exception("TaxInvoice not found.");

                //if (salTaxInvoice.PrintCount >= maxPrint)
                //    throw new Exception("Print max limit");

                //salTaxInvoice.PrintCount = salTaxInvoice.PrintCount == null ? salTaxInvoice.PrintCount = 1 : salTaxInvoice.PrintCount + 1;
                //salTaxInvoice.PrintBy = printby;
                //salTaxInvoice.PrintDate = DateTime.Now;

                //_taxInvoiceRepository.UpdateSalTaxInvoiceAsync(salTaxInvoice);



                await _unitOfWork.CompleteAsync();

                return salTaxInvoice;
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                // Do some logging stuff
                throw new Exception($"An error occurred when saving the taxinvoice: {strMessage}");
            }
        }

        public async Task<TaxInvoiceResponse> GetTaxInvoice2(TaxInvoiceRequest request)
        {
            try
            {
                //var maxPrint = 999;
                var salTaxInvoice = await _taxInvoiceRepository.GetTaxInvoice2Async(request);

                if (salTaxInvoice == null)
                    throw new Exception("TaxInvoice not found.");

                await _unitOfWork.CompleteAsync();

                return  salTaxInvoice;
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                // Do some logging stuff
                throw new Exception($"An error occurred when saving the taxinvoice: {strMessage}");
            }
        }


    }
}
