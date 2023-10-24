using Newtonsoft.Json;
using PostDay.API.Domain.Models.PostDay;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using PostDay.API.Domain.Repositories;
using PostDay.API.Domain.Services;
using MaxStation.Utility.Helpers.CRM;

namespace PostDay.API.Services
{
    public class CardStockService : ICardStockService
    {
        private readonly ICardStockRepository _cardStockRepository;
        private readonly ICrmService _crmService;

        public CardStockService(
            ICardStockRepository cardStockRepository, ICrmService crmService)
        {
            _cardStockRepository = cardStockRepository;
            _crmService = crmService;
        }

        public async Task<ResultModel> GetCardStock(CardStockRequest request)
        {
            var company = await _cardStockRepository.GetCompCode(request.CompanyCode);
            var url = await _cardStockRepository.GetSourceValue(request.OriShopCode, company);

            if (string.IsNullOrEmpty(url)) throw new Exception($"Data can't be found on branch {request.OriShopCode} or source value");

            try
            {
                var jwt = _crmService.GenerateJwt();
                var requestToString = JsonConvert.SerializeObject(request);
                var encryptText = _crmService.Encryption(requestToString);

                var response = await _crmService.SendApiAsync(jwt, url, encryptText);

                if (response.IsSuccessStatusCode)
                {
                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var content = await reader.ReadToEndAsync();
                        var decryption = _crmService.Decryption(content);
                        var responseBody = JsonConvert.DeserializeObject<CardStockResponse>(decryption);

                        return responseBody.ResultModel.FirstOrDefault() ?? new ResultModel();
                    }
                }

                throw new Exception($"Call api failed with url : {url}, Status Code : {response.StatusCode} and Reason : {response.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Call method GetCardStock failed with error message : {ex}");
            }
        }
    }
}
