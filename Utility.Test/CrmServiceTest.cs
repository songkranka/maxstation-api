using MaxStation.Utility.Helpers.CRM;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Utility.Test
{
    public class CrmServiceTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ICrmService _crmService;
        private IHttpClientFactory _clientFactory;

        public CrmServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;


            var crmSetting = Options.Create(new CrmSetting
            {
                Key = "626F9A17E540DF3DB4FA99166A0E03BCC9D944130CE29CCF50C68AE21D1ADCC4",
                Iv = "7B8E47125CA0160D7FDEDF82FA08C286",
                User = Convert.ToBase64String(Encoding.UTF8.GetBytes("99999999")),
                Role = Convert.ToBase64String(Encoding.UTF8.GetBytes("test"))
            });

            _crmService = new CrmService(crmSetting, null);
        }

        [Fact(DisplayName = "Generate crm jwt test")]
        public void GenerateJwtTest()
        {
            var jwt = _crmService.GenerateJwt();
            Assert.NotEmpty(_crmService.GenerateJwt());

            _testOutputHelper.WriteLine($"Token:=> {jwt}");
        }

        [Fact(DisplayName = "Create encryption and decryption test")]
        public void EncryptionAndDecryptionTest()
        {
            var plainText = "\test";
            var encrypt = _crmService.Encryption(plainText);
            Assert.NotEmpty(encrypt);
            _testOutputHelper.WriteLine($"Encrypt:=> {encrypt}");

            var decrypt = _crmService.Decryption(encrypt);
            Assert.NotEmpty(decrypt);
            _testOutputHelper.WriteLine($"Decrypt:=> {decrypt}");

            Assert.Equal(plainText, decrypt);
        }

    }
}