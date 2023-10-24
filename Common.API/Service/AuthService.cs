using Common.API.Domain.Repositories;
using Common.API.Domain.Service;
using MaxStation.Entities.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;

namespace Common.API.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AuthService(
            IAuthRepository authRepository,
            IUnitOfWork unitOfWork)
        {
            _authRepository = authRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MasEmployee> FindMasEmployeeByEmpCodeAsync(string empCode)
        {
            return await _authRepository.FindByEmpCodeAsync(empCode);
        }

        public async Task<AutEmployeeRole> FindAuthEmpRoleByEmpCode(string empCode)
        {
            return await _authRepository.FindAuthEmpRoleByEmpCode(empCode);
        }

        public async Task<MasBranch> FindBranchByBrnCode(string compCode, string empCode)
        {
            return await _authRepository.FindBranchByBrnCode(compCode, empCode);
        }

        public async Task<List<MasBranch>> GetBranchByCompCode(string compCode)
        {
            return await _authRepository.GetBranchByCompCode(compCode);
        }

        public async Task<List<MasBranch>> GetBranchByAuthCode(string compCode, int? authCode)
        {
            return await _authRepository.GetBranchByAuthCode(compCode, authCode);
        }

        public async Task<List<MasBranch>> GetAutBranchRole(string username, string compCode)
        {
            return await _authRepository.GetAutBranchRole(username, compCode);
        }

        public async Task<List<AutPositionRole>> GetAutPositionRole(string positionCode)
        {
            return await _authRepository.GetAutPositionRole(positionCode);
        }
        public async Task<MasPosition> GetMasPosition(string positionCode)
        {
            return await _authRepository.GetMasPosition(positionCode);
        }

        public bool LDapAuth(string username, string password)
        {
            var response = false;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://prd-ptg-apim.azure-api.net/ptwebservice/ldap_authen/PTWebService.asmx");
            request.Headers.Add("Apim-Subscription-Key", "ee875603f08b45b4862f4995ddd0567e");
            request.ContentType = "text/xml;charset=\"utf-8\"";
            request.Accept = "text/xml";
            request.Method = "POST";
            string jsonText = "<userID>" + username + "</userID>" + "<password>" + password + "</password>";
            ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            XmlDocument SOAPReqBody = new XmlDocument();
            SOAPReqBody.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
            <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">  
             <soap:Body>  
                <LDAP_HRIS_USER_AUTHEN2 xmlns=""http://tempuri.org/""> 
                   <userID>" + username + @"</userID>
                   <password>" + password + @"</password>
                </LDAP_HRIS_USER_AUTHEN2>  
             </soap:Body>  
            </soap:Envelope>");

            using (Stream stream = request.GetRequestStream())
            {
                SOAPReqBody.Save(stream);
            }

            using (WebResponse Serviceres = request.GetResponse())
            {
                using StreamReader rd = new StreamReader(Serviceres.GetResponseStream());
                string serviceResult = rd.ReadToEnd();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(serviceResult);

                XmlNodeList elemList = doc.GetElementsByTagName("LDAP_HRIS_USER_AUTHEN2Result");
                string jsonString = elemList[0].InnerXml;
                JObject jObjectData = JObject.Parse(jsonString);
                var isSuccessful = ((bool)jObjectData["isSuccessful"]);
                response = isSuccessful;
            }
            return response;
        }

        public async Task InsertLogLogin(LogLogin pLogin)
        {
            if(pLogin == null)
            {
                return;
            }
            await _authRepository.InsertLogLogin(pLogin);
            await _unitOfWork.CompleteAsync();
        }
    }
}
