using System.Net.Http;
using System.Threading.Tasks;

namespace MaxStation.Utility.Helpers.CRM
{
    public interface ICrmService
    {
        string GenerateJwt();
        string Encryption(string plainText);
        string Decryption(string encryptedText);
        Task<HttpResponseMessage> SendApiAsync(string jwt, string urlApi, string encryptedText);
    }
}