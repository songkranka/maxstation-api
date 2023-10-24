using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MaxStation.Utility.Helpers.CRM
{
    public class CrmService : ICrmService
    {
        private readonly CrmSetting _crmSetting;
        private readonly IHttpClientFactory _clientFactory;

        public CrmService(IOptions<CrmSetting> crmSetting, IHttpClientFactory clientFactory)
        {
            _crmSetting = crmSetting.Value;
            _clientFactory = clientFactory;
        }


        public static byte[] HexToByte(string hexStr)
        {
            byte[] bArray = new byte[hexStr.Length / 2];
            for (int i = 0; i < hexStr.Length / 2; i++)
            {
                byte firstNibble = byte.Parse(hexStr.Substring(2 * i, 1), NumberStyles.HexNumber); // [x,y)
                byte secondNibble = byte.Parse(hexStr.Substring(2 * i + 1, 1), NumberStyles.HexNumber);
                int finalByte = secondNibble | firstNibble << 4;
                bArray[i] = (byte)finalByte;
            }

            return bArray;
        }

        public static string FormatDateTimeStamp(DateTime datetime)
        {
            return datetime.ToString("yyyyMMddHHmmssfff", new CultureInfo("en-US"));
        }

        public string GenerateJwt()
        {
            string issuer = Guid.NewGuid().ToString();

            string audience = null;
            IEnumerable<Claim> claims = null;
            DateTime? notBefore = null;
            DateTime? expires = null;

            SymmetricSecurityKey key = new SymmetricSecurityKey(HexToByte(_crmSetting.Iv));
            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            JwtSecurityToken token = new JwtSecurityToken(issuer, audience, claims, notBefore, expires, signingCredentials);
            DateTime now = DateTime.Now;
            token.Payload["role"] = _crmSetting.RoleFromBase64;
            token.Payload["usr"] = _crmSetting.UserFromBase64;
            //server time
            //token.Payload["iat"] = FormatDateTimeStamp(now.AddHours(7));
            //token.Payload["exp"] = FormatDateTimeStamp(now.AddHours(7).AddMinutes(3));
            token.Payload["iat"] = FormatDateTimeStamp(now);
            token.Payload["exp"] = FormatDateTimeStamp(now.AddMinutes(3));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string Encryption(string plainText)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
            AeadParameters parameters = new AeadParameters(new KeyParameter(HexToByte(_crmSetting.Key)), 128, HexToByte(_crmSetting.Iv), null);

            cipher.Init(true, parameters);

            byte[] encryptedBytes = new byte[cipher.GetOutputSize(plainBytes.Length)];
            int retLen = cipher.ProcessBytes
                (plainBytes, 0, plainBytes.Length, encryptedBytes, 0);
            cipher.DoFinal(encryptedBytes, retLen);
            List<string> lstout = new List<string>();
            for (int x = 0; x < encryptedBytes.Length; x++)
            {
                lstout.Add(Convert.ToString(encryptedBytes[x], 2).PadLeft(8, '0'));
            }

            return Convert.ToBase64String(encryptedBytes, Base64FormattingOptions.None);
        }

        public string Decryption(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
            AeadParameters parameters = new AeadParameters(new KeyParameter(HexToByte(_crmSetting.Key)), 128, HexToByte(_crmSetting.Iv), null);

            cipher.Init(false, parameters);
            byte[] plainBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
            int retLen = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, plainBytes, 0);
            cipher.DoFinal(plainBytes, retLen);

            return Encoding.UTF8.GetString(plainBytes).TrimEnd("\r\n\0".ToCharArray());
        }

        public async Task<HttpResponseMessage> SendApiAsync(string jwt, string url, string encryptText)
        {
            try
            {
                var partnerId = _crmSetting.PartnerId;
                var reqKey = _crmSetting.ReqKey;

                var client = _clientFactory.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromMinutes(3);
                client.DefaultRequestHeaders.Add("PartnerId", partnerId);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
                client.DefaultRequestHeaders.Add("req-key", reqKey);
                var content = new StringContent(encryptText, Encoding.UTF8, "text/plain");
                var response = await client.PostAsync(url, content);

                return response;
            }

            catch (Exception ex)
            {
                throw new Exception($"Error URL SendingAPI : {url} And Error Message : {ex}");
            }
        }
    }
}