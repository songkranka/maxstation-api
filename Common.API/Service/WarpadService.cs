using Common.API.Domain.Models;
using Common.API.Domain.Repositories;
using Common.API.Domain.Services;
using MaxStation.Entities.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.API.Services
{
    public class WarpadService : IWarpadService
    {
        readonly IWarpadRepository _warpadRepositories;

        public WarpadService(IWarpadRepository warpadRepositories)
        {
            _warpadRepositories = warpadRepositories;
        }

        public Task<ResponseWarpadTaskList> GetWarpadTaskList(RequestWarpadTaskList req)
        {
            return _warpadRepositories.GetWarpadTaskList(req);
        }

        public async Task<ModelGetToDoTaskResult> GetToDoTask(string pStrEmpCode)
        {
            pStrEmpCode = (pStrEmpCode ?? string.Empty).Trim();
            if (0.Equals(pStrEmpCode.Length))
            {
                return null;
            }

            var urlWarpad = _warpadRepositories.GetSysConfigApi("Warpad", "M008");
            if (urlWarpad == null)
                return null;

            string strKey = "YegPfmseowTdfeoAeriq4E0o6Wdktvmw";
            string strIv = "0deTdeolsBtw9sLp";
            string strPlainText = "{\"userId\":\"" + pStrEmpCode + "\"}";
            string strEncrypt = encryptString(strPlainText, strKey, strIv);
            string strUrl = "https://stationwarpad.pt.co.th/authorize/api/Authen/generateToken";
            var dicHeader = new Dictionary<string, string>() {
                { "AuthData", strEncrypt }
            };
            string strJsonResult = await postData(strUrl, strPlainText, dicHeader);

            var tokenResult = JsonConvert.DeserializeObject<ModelGenerateTokenResult>(strJsonResult);
            string strAccessToken = (tokenResult?.resultData?.accessToken ?? string.Empty).Trim();
            if (strAccessToken.Length == 0)
            {                
                return null;
            }
            string strUrl2 = "https://stationwarpad.pt.co.th/tasks/api/TrnTask/getToDoTask";
            var dicHeader2 = new Dictionary<string, string>() {
                { "authorization", "Bearer " + strAccessToken }
            };
            string strPlainText2 = "{\"UserId\": \"" + pStrEmpCode + "\"}";
            string strJsonResult2 = await postData(strUrl2, strPlainText2, dicHeader2);
            var result = JsonConvert.DeserializeObject<ModelGetToDoTaskResult>(strJsonResult2);
            return result;
        }

        private string encryptString(string pStrInput, string pStrKey, string pStrIV)
        {
            byte[] arrResult;
            byte[] plainBytes = Encoding.UTF8.GetBytes(pStrInput);
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Key = Encoding.UTF8.GetBytes(pStrKey);
                aes.IV = Encoding.UTF8.GetBytes(pStrIV);
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (MemoryStream memoryStream = new MemoryStream())
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    arrResult = memoryStream.ToArray();
                }
            }
            string result = Convert.ToBase64String(arrResult);
            return result;
        }

        private async Task<string> postData(string pStrUrl, string pStrJsonBody, Dictionary<string, string> pDicHeader)
        {
            pStrUrl = (pStrUrl ?? string.Empty).Trim();
            if (0.Equals(pStrUrl.Length))
            {
                return string.Empty;
            }
            var uri = new Uri(pStrUrl);
            var jsonHeader = new MediaTypeWithQualityHeaderValue("application/json");
            pStrJsonBody = (pStrJsonBody ?? string.Empty).Trim();
            string result = string.Empty;
            using (var jsonBody = new StringContent(pStrJsonBody, Encoding.UTF8, "application/json"))
            using (var client = new HttpClient())
            {
                if (pDicHeader != null && pDicHeader.Any())
                {
                    foreach (var strKey in pDicHeader.Keys)
                    {
                        client.DefaultRequestHeaders.Add(strKey, pDicHeader[strKey]);
                    }
                }
                client.DefaultRequestHeaders.Accept.Add(jsonHeader);
                var response = await client.PostAsync(uri, jsonBody);
                if (response != null)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            return result;

        }
    }
}
