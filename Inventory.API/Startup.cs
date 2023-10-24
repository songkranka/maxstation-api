using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Inventory.API.Controllers.Audit;
using Inventory.API.Controllers.Config;
using Inventory.API.Domain.Repositories;
using Inventory.API.Domain.Services;
using Inventory.API.Extensions;
using Inventory.API.Repositories;
using Inventory.API.Services;
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

namespace Inventory.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            environment = env;
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

            //services.AddControllers()
            //.AddJsonOptions(options =>
            //   options.JsonSerializerOptions.PropertyNamingPolicy = null);

            #region DBConnect
            var dbconnection = "";
            var ConnectionFile = Environment.GetEnvironmentVariable("ConnectionFile");
            var ConnectionString = Environment.GetEnvironmentVariable("DB_ConnectionString");
            //-------- Kuber ------------
            // try
            // {
            //     if (ConnectionFile != null)
            //     {
            //         FileStream fileStream = new FileStream(ConnectionFile, FileMode.Open);
            //         using (StreamReader reader = new StreamReader(fileStream))
            //         {
            //             dbconnection = reader.ReadLine();
            //         }
            //     }
            //     else
            //     {
            //         var KeyVault = Environment.GetEnvironmentVariable("KeyVault");
            //         var SecretKey = Environment.GetEnvironmentVariable("SecretKey");
            //         if (KeyVault != null)
            //         {
            //             var secretClient = new SecretClient(new Uri(KeyVault), new DefaultAzureCredential());
            //             KeyVaultSecret secret = secretClient.GetSecret(SecretKey);
            //             dbconnection = secret.Value;
            //         }
            //         if (!environment.IsProduction())
            //         {
            //             dbconnection = Configuration.GetConnectionString("DefaultConnection");//change conect {DefaultConnection,DevelopConnection}
            //         }
            //     }
            // }
            // catch (Exception ex)
            // {
            //     throw new Exception(ex.Message);
            // }
           // services.AddDbContext<PTMaxstationContext>(options => options.UseSqlServer(dbconnection));
            if (!environment.IsProduction())
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
            services.AddScoped<IAdjustService, AdjustService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IAdjustRequestService, AdjustRequestService>();
            services.AddScoped<ITransferOutService, TransOutService>();
            services.AddScoped<IWithdrawService, WithdrawService>();
            services.AddScoped<ITransferInService, TransferInService>();
            services.AddScoped<IReceiveGasService, ReceiveGasService>();
            services.AddScoped<IReceiveOilService, ReceiveOilService>();
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            services.AddScoped<ISupplyRequestService, SupplyRequestService>();

            services.AddScoped<IAuditService, AuditService>();
            #endregion

            #region Repositories
            services.AddScoped<IAdjustRepository, AdjustRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IAdjustRequestRepository, AdjustRequestRepository>();
            services.AddScoped<ITransferOutRepository, TransferOutRepository>();
            services.AddScoped<IWithdrawRepository, WithdrawRepository>();
            services.AddScoped<ITransferInRepository, TransferInRepository>();
            services.AddScoped<IReceiveGasRepository, ReceiveGasRepository>();
            services.AddScoped<IReceiveOilRepository, ReceiveOilRepository>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<ISupplyRequestRepository, SupplyRequestRepository>();
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
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor accessor)
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

            //app.UseCors(x => x
            //.AllowAnyOrigin()
            //.AllowAnyMethod()
            //.AllowAnyHeader());
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
