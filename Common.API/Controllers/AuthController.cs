using AutoMapper;
using Common.API.Domain.Models;
using Common.API.Domain.Response;
using Common.API.Domain.Service;
using Common.API.Resource.Auth;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Common.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthController));
        private readonly IMapper _mapper;

        public AuthController(
            IAuthService authService,
            PTMaxstationContext context,
            IMapper mapper) : base(context)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("Login")]
        //[HttpPost("Login"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Login([FromBody] LoginModel user)
        {
            try
            {
                var branches = new List<MasBranch>();
                var roles = new List<AutPositionRole>();
                var position = new MasPosition();
                var branchResource = new List<BranchResource>();

                if (user == null)
                    return BadRequest("Isvalid client request");

                var masEmployee = await _authService.FindMasEmployeeByEmpCodeAsync(user.UserName);
                if (masEmployee == null)
                {
                    return BadRequest("ไม่พบรหัสพนักงานในระบบ");
                }
                else
                {
                    var authEmployee = await _authService.FindAuthEmpRoleByEmpCode(user.UserName);

                    if (authEmployee == null)
                    {
                        return BadRequest("ไม่พบสิทธิการใช้งานระบบ");
                    }
                    else
                    {
                        switch (authEmployee.AuthCode)
                        {
                            case 0:
                                var branch = await _authService.FindBranchByBrnCode(user.CompCode, user.UserName);
                                branches.Add(branch);
                                break;
                            case 1:
                                branches = await _authService.GetBranchByCompCode(user.CompCode);
                                break;
                            default:
                                branches = await _authService.GetBranchByAuthCode(user.CompCode, authEmployee.AuthCode);
                                break;
                        }

                        //if (branches.Any(x => x == null))
                        if (branches.Count == 0)
                        {
                            return BadRequest("ไม่พบสิทธิการเข้าใช้งานสาขา");
                        }
                        else
                        {
                            branchResource = _mapper.Map<List<MasBranch>, List<BranchResource>>(branches);
                        }
                    }

                    roles = await _authService.GetAutPositionRole(authEmployee.PositionCode);
                    //position = await _authService.GetMasPosition(authEmployee.PositionCode);

                    var lDapAuth = _authService.LDapAuth(user.UserName, user.Password);
                    if (lDapAuth)
                    {
                        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        //new Claim(ClaimTypes.Role, position.PositionName)
                        new Claim(ClaimTypes.Role, "General")
                    };


                        var tokenOptions = new JwtSecurityToken(
                            issuer: "Develop",
                            audience: "Develop",
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(120),
                            signingCredentials: signinCredentials
                        );

                        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                        var logLogin = new LogLogin();
                        logLogin.CompCode = user.CompCode;
                        logLogin.CreatedBy = user.UserName;
                        logLogin.CreatedDate = DateTime.Now;
                        logLogin.EmpCode = user.UserName;
                        logLogin.IpAddress = user.IpAddress;
                        await _authService.InsertLogLogin(logLogin);

                        return Ok(new AuthenticatedResponse { Token = tokenString, Branches = branchResource, PositionRoles = roles });

                    }
                }
                
                return StatusCode((int)HttpStatusCode.Unauthorized, "รหัสพนักงานหรือรหัสผ่านไม่ถูกต้อง");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        //[HttpGet("GenerateToken")]
        //public IActionResult GenerateToken()
        //{
        //    try
        //    {
        //        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
        //        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        //        var tokeOptions = new JwtSecurityToken(
        //            issuer: "Develop",
        //            audience: "Develop",
        //            claims: new List<Claim>(),
        //            expires: DateTime.Now.AddMinutes(15),
        //            signingCredentials: signinCredentials
        //        );
        //        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        //        return Ok(new AuthenticatedResponse { Token = tokenString });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpPost("GenerateToken")]
        public IActionResult GenerateToken([FromHeader]string ProgramId)
        {
            // Console.WriteLine($"ProgramId:=================> {ProgramId}");
            try
            {
                string strMd5 = CreateMD5("MaxStation");
                if(strMd5.ToUpper() != ProgramId.ToUpper())
                {
                    return Ok("Invalid ProgramId");
                }


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
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                //return Ok(new AuthenticatedResponse { Token = tokenString });
                return Ok(tokenString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                //return Convert.ToHexString(hashBytes); // .NET 5 +

                // Convert the byte array to hexadecimal string prior to .NET 5
                StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok("Hello World");
        }
    }
}
