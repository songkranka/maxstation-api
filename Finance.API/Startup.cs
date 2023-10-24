using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Finance.API.Controllers.Config;
using Finance.API.Domain.Repositories;
using Finance.API.Domain.Services;
using Finance.API.Extensions;
using Finance.API.Helpers;
using Finance.API.Helpers.Setting;
using Finance.API.Persistence.Context;
using Finance.API.Repositories;
using Finance.API.Services;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAutoMapper(typeof(Startup));
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                // Adds a custom error response factory when ModelState is invalid
                options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory.ProduceErrorResponse;
            });

            #region DBConnect
            var dbconnection = "";
            //-------- Kuber ------------
            var ConnectionFile = Environment.GetEnvironmentVariable("ConnectionFile");
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
            //         var KeyVault = Environment.GetEnvironmentVariable("KeyVault");
            //         var SecretKey = Environment.GetEnvironmentVariable("SecretKey");
            //         if (KeyVault != null)
            //         {
            //             var secretClient = new SecretClient(new Uri(KeyVault), new DefaultAzureCredential());
            //             KeyVaultSecret secret = secretClient.GetSecret(SecretKey);
            //             dbconnection = secret.Value;
            //         }
            //         if (!HostEnvironment.IsProduction())
            //         {
            //             dbconnection = Configuration.GetConnectionString("DefaultConnection");//change conect {DefaultConnection,DevelopConnection}
            //         }

            //     }
            //     catch (Exception ex)
            //     {
            //         throw new Exception(ex.Message);
            //     }

            // }

            // services.AddDbContext<PTMaxstationContext>(options => options.UseSqlServer(dbconnection));
             if (!HostEnvironment.IsProduction())
            {
                dbconnection = Configuration.GetConnectionString("DefaultConnection");//change conect {DefaultConnection,DevelopConnection}

            }else{
                dbconnection = ConnectionString;
            }
            // services.AddDbContext<PTMaxstationContext>(options => options.UseSqlServer(dbconnection));
            services.AddDbContext<PTMaxstationContext>(options => options.UseSqlServer(dbconnection));
            #endregion

            #region DBConnectFinance
            var maxStationConnection = string.Empty;
            maxStationConnection = Configuration.GetConnectionString("MaxStationConnection");//change conect {DefaultConnection,DevelopConnection}
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(maxStationConnection));
            #endregion

            #region Services
            services.AddScoped<IReceiveService, ReceiveService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IHttpClientService, HttpClientService>();
            services.Configure<JwtSetting>(Configuration.GetSection(nameof(JwtSetting)));
            #endregion

            #region Repositories
            services.AddScoped<IReceiveRepository, ReceiveRepository>();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<ITaxInvoiceRepository, TaxInvoiceRepository>();
            services.AddScoped<ICreditSaleRepository, CreditSaleRepository>();
            services.AddScoped<IConfigApiRepository, ConfigApiRepository>();
            #endregion

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
            services
                .AddHttpClient()
                .AddHttpContextAccessor();
            services.AddCustomSwagger();
            services.AddAutoMapper(typeof(Startup));

            services.AddHealthChecks()
            .AddCheck<ApiHealthCheck>("api")
            .AddCheck<SecondaryHealthCheck>("secondary");

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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCustomSwagger();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
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
