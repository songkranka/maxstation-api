using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Report.API.Controllers.Config;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using Report.API.Extensions;
using Report.API.Helpers;
using Report.API.Repositories;
using Report.API.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.API
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

            #region DBConnectv
            var dbconnection = "";
            //-------- Kuber ------------
            var ConnectionFile = Environment.GetEnvironmentVariable("ConnectionFile");
            var ConnectionString = Environment.GetEnvironmentVariable("DB_ConnectionString");
            if (ConnectionFile != null)
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
            #endregion

            #region OracleConnect
            //var orclConnection = "";
            //try
            //{
            //    if (HostEnvironment.IsProduction())
            //    {
            //        orclConnection = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=172.16.112.207)(PORT=1524))(CONNECT_DATA=(SERVICE_NAME=orapos)));Persist Security Info=True;User Id=raptorpos;Password=raptor18;"; //dev                    
            //    }
            //    else
            //    {
            //        orclConnection = Configuration.GetConnectionString("OracleDbConnection");//change conect {DefaultConnection,DevelopConnection}
            //    }

            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}

            //Action<POSConnection> posConnect = (opt =>
            //{
            //    opt.ConnectionString = orclConnection;
            //});
            //services.Configure(posConnect);
            //services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<POSConnection>>().Value);
            #endregion

            #region Services
            services.AddScoped<ITaxInvoiceService, TaxInvoiceService>();
            services.AddScoped<IDeliveryCtrlService, DeliveryCtrlService>();
            services.AddScoped<IReportSummaryOilBalanceService, ReportSummaryOilBalanceService>();
            services.AddScoped<IReportSummarySaleService, ReportSummarySaleService>();
            services.AddScoped<IReportStockService, ReportStockService>();
            services.AddScoped<IReceivePayService, ReceivePayService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IReportAuditService, ReportAuditService>();
            services.AddScoped<IGovermentService, GovermentService>();
            services.AddScoped<IVatSaleService, VatSaleService>();
            services.AddScoped<IStationService, StationService>();
            services.AddScoped<IMeterService, MeterService>();
            services.AddScoped<IWithdrawService, WithdrawService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IPostDayService,PostDayService>();
            services.AddScoped<ISalesService, SalesService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IFinanceService, FinanceService>();
            services.AddScoped<IBillingService, BillingService>();
            services.AddScoped<ICreditNoteService, CreditNoteService>();
            #endregion

            #region Repositories
            services.AddScoped<ITaxInvoiceRepository, TaxInvoiceRepository>();
            services.AddScoped<IDeliveryCtrlRepository, DeliveryCtrlRepository>();
            services.AddScoped<IReportSummaryOilBalanceRepository, ReportSummaryOilBalanceRepository>();
            services.AddScoped<IReportSummarySaleRepository, ReportSummarySaleRepository>();
            services.AddScoped<IReportStockRepository, ReportStockRepository>();
            services.AddScoped<IReceivePayRepository, ReceivePayRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReportAuditRepository, ReportAuditRepository>();
            services.AddScoped<IGovermentRepository, GovermentRepository>();
            services.AddScoped<IVatSaleRepository, VatSaleRepository>();
            services.AddScoped<IStationRepository, StationRepository>();
            services.AddScoped<IMeterRepository, MeterRepository>();
            services.AddScoped<IWithdrawRepository, WithdrawRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IPostDayRepository, PostDayRepository>();
            services.AddScoped<ISalesRepository, SalesRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IFinanceRepository, FinanceRepository>();
            services.AddScoped<IBillingRepository, BillingRepository>();
            services.AddScoped<ICreditNoteRepository, CreditNoteRepository>();
            #endregion

            services.AddScoped<IUnitOfWork, UnitOfWork>();
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
