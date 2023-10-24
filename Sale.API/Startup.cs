using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sale.API.Controllers.Config;
using Sale.API.Domain.Repositories;
using Sale.API.Domain.Services;
using Sale.API.Extensions;
using Sale.API.Repositories;
using Sale.API.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.API
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
            services.AddMemoryCache();
            services.AddCustomSwagger();
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
            if (!HostEnvironment.IsProduction())
            {
                dbconnection = Configuration.GetConnectionString("DefaultConnection");//change conect {DefaultConnection,DevelopConnection}

            }else{
                dbconnection = ConnectionString;
            }
            // services.AddDbContext<PTMaxstationContext>(options => options.UseSqlServer(dbconnection));
            services.AddDbContext<PTMaxstationContext>(options => options.UseSqlServer(dbconnection));
            LogErrorService.SetConnectionString(dbconnection);
            #endregion

            #region Services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICashSaleService, CashSaleService>();
            services.AddScoped<IQuotationService, QuotationService>();
            services.AddScoped<ICreditSaleService, CreditSaleService>();
            services.AddScoped<ICashTaxService, CashTaxService>();
            services.AddScoped<ICreditNoteService, CreditNoteService>();
            #endregion

            #region Repositories           
            services.AddScoped<ICashSaleRepository, CashSaleRepository>();
            services.AddScoped<IQuotationRepository, QuotationRepository>();
            services.AddScoped<ICreditSaleRepository, CreditSaleRepository>();
            services.AddScoped<ICashTaxRepository, CashTaxRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<ICreditNoteRepository, CreditNoteRepository>();
            #endregion


            #region Log4Net
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            //var loggingOptions = config.GetSection("Log4NetCore").Get<Log4NetProviderOptions>();
            //services.AddLogging(configure => configure.AddLog4Net(loggingOptions));
            #endregion
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
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor accessor)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCustomSwagger(env);
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
            app.Use(async (context, next) =>
            {
                await LogErrorService.SetHttpContext(context);
                await next();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = WriteHealthCheckResponse
            });
            LogErrorService.SetHttpContextAccessor(accessor);
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
