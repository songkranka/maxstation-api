using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using MasterData.API.Controllers.Config;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MasterData.API.Extensions;
using MasterData.API.Repositories;
using MasterData.API.Services;
using MaxStation.Entities.Models;
using MaxStation.Utility.Helpers.CollectLogError;
using MaxStation.Utility.LogConfig;
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
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.api
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
            //             dbconnection = Configuration.GetConnectionString("DefaultConnection"); //change conect {DefaultConnection,DevelopConnection}
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
            //services.AddDbContext<PTMaxstationContext>(options =>
            //{
            //    options.UseSqlServer(dbconnection);
            //    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            //});

            services.AddDbContext<PTMaxstationContext>(options => options.UseSqlServer(dbconnection));

            #endregion

            #region Service

            services.AddScoped<ISoapPTWebServiceApi, SoapPTWebServiceApi>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductGroupService, ProductGroupService>();
            services.AddScoped<IProductUnitService, ProductUnitService>();
            services.AddScoped<IReasonService, ReasonService>();
            services.AddScoped<IBankService, BankService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IOtherService, OtherService>();
            services.AddScoped<IMasControlService, MasControlService>();
            services.AddScoped<IDocumentTypeService, DocumentTypeService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerCarService, CustomerCarService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IPriceService, PriceService>();
            services.AddScoped<ICostCenterService, CostCenterService>();
            services.AddScoped<IDropdownService, DropdownService>();
            services.AddScoped<ICompanyCarService, CompanyCarService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IUnlockService, UnlockService>();
            services.AddScoped<IUnitService, UnitService>();

            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IEmployeeAuthService, EmployeeAuthService>();
            services.AddScoped<IMasMappingService, MasMappingService>();
            services.AddScoped<IMasPositionService, MasPositionService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IDocpatternService, DocpatternService>();
            services.AddScoped<ILogErrorService, LogErrorService>();

            #endregion

            #region repositories

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductGroupRepository, ProductGroupRepository>();
            services.AddScoped<IProductUnitRepository, ProductUnitRepository>();
            services.AddScoped<IReasonRepository, ReasonRepository>();
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IOtherRepository, OtherRepository>();
            services.AddScoped<IMasControlRepository, MasControlRepository>();
            services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerCarRepository, CustomerCarRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IPriceRepository, PriceRepository>();
            services.AddScoped<ICostCenterRepository, CostCenterRepository>();
            services.AddScoped<ICompanyCarRepository, CompanyCarRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IUnlockRepository, UnlockRepository>();
            services.AddScoped<IEmployeeAuthenRepository, EmployeeAuthenRepository>();
            services.AddScoped<IMasBranchConfigRepository, MasBranchConfigRepository>();
            services.AddScoped<IEmployeeAuthRepository, EmployeeAuthRepository>();
            services.AddScoped<IMasMappingRepository, MasMappingRepository>();
            services.AddScoped<IMasPositionRepository, MasPositionRepository>();
            services.AddScoped<IDocpatternRepository, DocpatternRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();

            #endregion

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddCustomSwagger();
            services.AddAutoMapper(typeof(Startup));


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

            services.AddHttpClient().AddHttpContextAccessor();

            services.Log4NetAdoAppenderRegister(Configuration);
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

            //app.UseCors(x => x
            //.AllowAnyOrigin()
            //.AllowAnyMethod()
            //.AllowAnyHeader());
            app.UseCors("EnableCORS");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
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