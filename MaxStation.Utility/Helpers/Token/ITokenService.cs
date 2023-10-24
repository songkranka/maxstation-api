using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MaxStation.Utility.Helpers.Token
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync();
    }
}
