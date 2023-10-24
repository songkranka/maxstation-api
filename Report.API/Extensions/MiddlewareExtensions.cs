using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Report API",
                    Version = "v1",
                    Description = "RESTful API built with ASP.NET Core 3.1 to show restful services.",
                    Contact = new OpenApiContact
                    {
                        Name = "PTG",
                        Url = new Uri("https://www.ptgenergy.co.th/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Developer",
                    },
                });
                cfg.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer"
                });
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //cfg.IncludeXmlComments(xmlPath);
            });
            return services;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
        {
            var rootUrl = Environment.GetEnvironmentVariable("ROOT_URL");
            //var path = (!env.IsProduction()) ? "" : "/report";
            app.UseSwagger().UseSwaggerUI(options =>
            {
                //options.SwaggerEndpoint("/swagger/v1/swagger.json", "Report API");
                //options.DocumentTitle = "Report API";

                options.SwaggerEndpoint($"{rootUrl}/swagger/v1/swagger.json", "Report API");
                options.DocumentTitle = $"Report API";
            });
            return app;
        }
    }
}
