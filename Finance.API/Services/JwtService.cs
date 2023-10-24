using Finance.API.Domain.Services;
using Finance.API.Helpers.Setting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Finance.API.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSetting _jwtSetting;

        public JwtService(IOptions<JwtSetting> jwtSetting)
        {
            _jwtSetting = jwtSetting.Value;
        }

        public string GenerateJwt()
        {
            //string issuer = Guid.NewGuid().ToString();

            //string audience = null;
            //IEnumerable<Claim> claims = null;
            //DateTime? notBefore = null;
            //DateTime? expires = null;

            //SymmetricSecurityKey key = new SymmetricSecurityKey(HexToByte(_jwtSetting.Iv));
            //SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            //JwtSecurityToken token = new JwtSecurityToken(issuer, audience, claims, notBefore, expires, signingCredentials);
            //DateTime now = DateTime.Now;
            //token.Payload["role"] = _jwtSetting.RoleFromBase64;
            //token.Payload["usr"] = _jwtSetting.UserFromBase64;
            //token.Payload["iat"] = FormatDateTimeStamp(now);
            //token.Payload["exp"] = FormatDateTimeStamp(now.AddMinutes(3));
            //return new JwtSecurityTokenHandler().WriteToken(token);

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,"MaxStation")
            };

            var tokeOptions = new JwtSecurityToken(
                issuer: "Develop",
                audience: "Develop",
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: signinCredentials
            );
            return  new JwtSecurityTokenHandler().WriteToken(tokeOptions);
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
    }
}
