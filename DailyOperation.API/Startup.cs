using Autofac;
using Autofac.Extensions.DependencyInjection;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DailyOperation.API.Controllers.Config;
using DailyOperation.API.Domain.Repositories;
using DailyOperation.API.Domain.Services;
using DailyOperation.API.Extensions;
using DailyOperation.API.Helpers;
using DailyOperation.API.Repositories;
using DailyOperation.API.Services;
using MaxStation.Entities.Models;
using MaxStation.Utility.Caches;
using MaxStation.Utility.Extensions;
//using MaxStation.Utility.Caches;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyOperation.API
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
            //var ConnectionString = ;
            //-------- Kuber ------------
            var ConnectionString = Environment.GetEnvironmentVariable("DB_ConnectionString");
            try
            {
                var ConnectionFile = Environment.GetEnvironmentVariable("ConnectionFile");
                Console.WriteLine("===========================================.Hello World!111");
                Console.WriteLine("===========================================.Hello World!2222"+$"{ConnectionString}");
// var KeyVault = Environment.GetEnvironmentVariable("KeyVault");
// var SecretKey = Environment.GetEnvironmentVariable("SecretKey");
                var KeyVault = Configuration.GetValue<string>("KeyVault");
                var SecretKey = Configuration.GetValue<string>("SecretKey");
                
                Console.WriteLine("===========================================.Hello World!"+$"{KeyVault}");
                Console.WriteLine("===========================================.Hello World!" + $"{SecretKey}");
                if (ConnectionString != "")
                {
                    dbconnection = ConnectionString;
                }
                else
                {
                    //var KeyVault = Environment.GetEnvironmentVariable("KeyVault");
                    //var SecretKey = Environment.GetEnvironmentVariable("SecretKey");
                    if (KeyVault != null)
                    {
                        var secretClient = new SecretClient(new Uri(KeyVault), new DefaultAzureCredential());
                        KeyVaultSecret secret = secretClient.GetSecret(SecretKey);
                        dbconnection = secret.Value;
                    }
                    if (!HostEnvironment.IsProduction())
                    {
                        dbconnection = Configuration.GetConnectionString("DefaultConnection");//change conect {DefaultConnection,DevelopConnection}
                    }
                }
                /*if (ConnectionFile != null)
                {
                    FileStream fileStream = new FileStream(ConnectionFile, FileMode.Open);
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        dbconnection = reader.ReadLine();
                    }
                }
                else
                {
                    var KeyVault = Environment.GetEnvironmentVariable("KeyVault");
                    var SecretKey = Environment.GetEnvironmentVariable("SecretKey");
                    if (KeyVault != null)
                    {
                        var secretClient = new SecretClient(new Uri(KeyVault), new DefaultAzureCredential());
                        KeyVaultSecret secret = secretClient.GetSecret(SecretKey);
                        dbconnection = secret.Value;
                    }
                    if (!HostEnvironment.IsProduction())
                    {
                        dbconnection = Configuration.GetConnectionString("DefaultConnection");//change conect {DefaultConnection,DevelopConnection}
                    }
                }*/
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            //dbconnection = Configuration.GetConnectionString(ConnectionString);
            services.AddDbContext<PTMaxstationContext>(options => options.UseSqlServer(dbconnection));
            LogErrorService.SetConnectionString(dbconnection);
            #endregion


            #region OracleConnect
            var orclConnection = "";
            try
            {

                //var orclKeyVault = Environment.GetEnvironmentVariable("KeyVaultOracle");
                //var orclSecretKey = Environment.GetEnvironmentVariable("SecretKeyOracle");

                //    if (orclKeyVault != null)
                //    {
                //        var secretClient = new SecretClient(new Uri(orclKeyVault), new DefaultAzureCredential());
                //        KeyVaultSecret secret = secretClient.GetSecret(orclSecretKey);
                //        orclConnection = secret.Value;
                //    }

                if (HostEnvironment.IsProduction())
                {
                    orclConnection = Environment.GetEnvironmentVariable("OracleDbConnection");
                }
                else
                {
                    orclConnection = Configuration.GetConnectionString("OracleDbConnection");//change conect {DefaultConnection,DevelopConnection}                                        
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            //orclConnection = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=172.16.100.180)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orapos)));Persist Security Info=True;User Id=raptorpos;Password=raptor18;";//prod
            Action<POSConnection> posConnect = (opt =>
            {
                opt.ConnectionString = orclConnection;
            });
            services.Configure(posConnect);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<POSConnection>>().Value);
            #endregion


            #region Services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPosService, PosService>();
            services.AddScoped<IMeterService, MeterService>();
            services.AddScoped<IQueueService, QueueService>();

            //services.AddHttpClient<IPosService, PosService>();
            #endregion

            #region Repositories
            services.AddScoped<IPosRepository, PosRepository>();
            services.AddScoped<IMeterRepository, MeterRepository>();
            services.AddHttpClient<IPosRepository, PosRepository>();
            services.AddHttpClient<IQueueRepository, QueueRepository>();
            #endregion


            #region CacheMemory

            services.AddMemoryCache();
            services.Configure<CacheOption>(Configuration.GetSection(nameof(CacheOption)));
            services.AddScoped<ICommonCacheHelper, CommonCacheHelper>();

            #endregion

            //List<string> envs = new List<string>();
            //foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            //{                
            //    envs.Add($"Key {de.Key}; Value = {1}");
            //}
            //strtest = JsonConvert.SerializeObject(envs);


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
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedProto
            //});
        }


        private  Task WriteHealthCheckResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";
            var json = new JObject(
            new JProperty("status", result.Status.ToString()),
            new JProperty("results", new JObject(result.Entries.Select(pair =>
            new JProperty(pair.Key, new JObject(
            //new JProperty("status", $"{pair.Value.Status}, Key: [{Configuration.GetValue<string>("KeyVault")}]"),
            new JProperty("status", $"{pair.Value.Status}, Key: [{Configuration.GetValue<string>("KeyVault")}]"),
            new JProperty("description", pair.Value.Description),
            new JProperty("data", new JObject(pair.Value.Data.Select(
            p => new JProperty(p.Key, p.Value))))))))));
            return httpContext.Response.WriteAsync(
            json.ToString(Formatting.Indented));
        }

    }
}
