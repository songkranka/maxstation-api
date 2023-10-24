using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Services;
using Newtonsoft.Json;
using PTWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class SoapPTWebServiceApi : ISoapPTWebServiceApi
    {
        //public readonly string serviceUrl = "http://supply.pt.co.th/PTWebService/PTWebService.asmx?WSDL";
        public readonly string serviceUrl = "http://www.pt.co.th/PTWebService/PTWebService.asmx?WSDL";
        public readonly EndpointAddress endpointAddress;
        public readonly BasicHttpBinding basicHttpBinding;

        public SoapPTWebServiceApi()
        {
            endpointAddress = new EndpointAddress(serviceUrl);

            //basicHttpBinding = new BasicHttpBinding();
            ////basicHttpBinding.ProxyAddress = new Uri(string.Format("http://{0}:{1}", proxyAddress, proxyPort));
            //basicHttpBinding.UseDefaultWebProxy = false;
            //basicHttpBinding.Security.Mode = BasicHttpSecurityMode.Transport;
            //basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            //basicHttpBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Basic;

            basicHttpBinding = new BasicHttpBinding(endpointAddress.Uri.Scheme.ToLower() == "http" ? BasicHttpSecurityMode.None : BasicHttpSecurityMode.Transport)
            {
                OpenTimeout = TimeSpan.MaxValue,
                CloseTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                SendTimeout = TimeSpan.MaxValue
            };
            
        }

        public async Task<PTWebServiceSoapClient> GetInstanceAsync()
        {
            //var authenticationClient = new PTWebServiceSoapClient(basicHttpBinding, endpointAddress);
            ////authenticationClient.ClientCredentials.UserName.UserName = username;
            ////authenticationClient.ClientCredentials.UserName.Password = password;
            //return await Task.Run(() => authenticationClient);
            return await Task.Run(() => new PTWebServiceSoapClient(basicHttpBinding, endpointAddress));
        }

        public async Task<OilPrice> GetOilPriceByBrn(OilPriceRequest request)
        {
            var client = await GetInstanceAsync();
            var oilPrices = await client.CRM_GetOilPriceListByBrnAsync(request.brnCode, DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss"));
            var result = JsonConvert.DeserializeObject<OilPrice>(oilPrices);
            return result;
        }
    }
}
