using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using Common.API.Domain.Repositories;
using Common.API.Domain.Services;
using Common.API.Services;
using Common.API.Repositories;
using Common.API.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Linq;
using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Common.API.Domain.Service;
using Common.API.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace common.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            environment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region DBConnect
            var dbconnection = "";
            //-------- Kuber ------------
            // var ConnectionFile = Environment.GetEnvironmentVariable("ConnectionFile");
            var ConnectionString = Environment.GetEnvironmentVariable("DB_ConnectionString");
            // if (ConnectionFile != null)
            // {
            //     FileStream fileStream = new FileStream(ConnectionFile, FileMode.Open);
            //     using (StreamReader reader = new StreamReader(fileStream))
            //     {
            //         dbconnection = reader.ReadLine();
            //     }
            // }
            // else
            // {
            //     try
            //     {
            //         dbconnection = Configuration.GetConnectionString(ConnectionString);//change conect {DefaultConnection,DevelopConnection}
            //                                                                            // var KeyVault = Environment.GetEnvironmentVariable("KeyVault");
            //                                                                            // var SecretKey = Environment.GetEnvironmentVariable("SecretKey");
            //                                                                            // if (KeyVault != null)
            //                                                                            // {
            //                                                                            //     var secretClient = new SecretClient(new Uri(KeyVault), new DefaultAzureCredential());
            //                                                                            //     KeyVaultSecret secret = secretClient.GetSecret(SecretKey);
            //                                                                            //     dbconnection = secret.Value;
            //                                                                            // }
            //                                                                            // if (!environment.IsProduction())
            //                                                                            // {
            //                                                                            //     dbconnection = Configuration.GetConnectionString("DefaultConnection");//change conect {DefaultConnection,DevelopConnection}

            //         // }
            //     }
            //     catch (Exception ex)
            //     {
            //         throw new Exception(ex.Message);
            //     }

            // }
            if (!environment.IsProduction())
            {
                dbconnection = Configuration.GetConnectionString("DefaultConnection");//change conect {DefaultConnection,DevelopConnection}

            }
            else
            {
                dbconnection = ConnectionString;
            }
            // dbconnection = Configuration.GetConnectionString(ConnectionString);//change conect {DefaultConnection,DevelopConnection}
            // services.AddDbContext<PTMaxstationContext>(options => options.UseSqlServer(dbconnection));
            services.AddDbContext<PTMaxstationContext>(options => options.UseSqlServer(dbconnection));

            

            #endregion

            #region Log4Net
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            //var loggingOptions = config.GetSection("Log4NetCore").Get<Log4NetProviderOptions>();
            // add logging
            //services.AddLogging(configure => configure.AddLog4Net(loggingOptions));
            #endregion

            #region Services
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IWarpadService, WarpadService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ICashSaleService, CashSaleService>();
            services.AddScoped<IAuthService, AuthService>();
            #endregion

            #region Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICommonRepository, CommonRepository>();
            services.AddScoped<IWarpadRepository, WarpadRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<ICashSaleRepository, CashSaleRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            #endregion

            services.AddCustomSwagger();
            services.AddControllers();
            services.AddHealthChecks()
            .AddCheck<ApiHealthCheck>("api")
            .AddCheck<SecondaryHealthCheck>("secondary");

            services.AddAutoMapper(typeof(Startup));

            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            var issuers = new List<string>()
            {
                "localhost",
                "Develop",
                "UAT",
                "Production"
            };

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //ValidateIssuer = true,
                    //ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuers = issuers,
                    ValidAudiences = issuers,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseCustomSwagger();
            }

            app.UseCors("EnableCORS");

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = WriteHealthCheckResponse
            });
        }

        private static Task WriteHealthCheckResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";
            var json = new JObject(
            new JProperty("status", result.Status.ToString()),
            new JProperty("results", new JObject(result.Entries.Select(pair =>
            new JProperty(pair.Key, new JObject(
            new JProperty("status", pair.Value.Status.ToString()),
            new JProperty("description", pair.Value.Description),
            new JProperty("data", new JObject(pair.Value.Data.Select(
            p => new JProperty(p.Key, p.Value))))))))));
            return httpContext.Response.WriteAsync(
            json.ToString(Formatting.Indented));
        }
    }
}
