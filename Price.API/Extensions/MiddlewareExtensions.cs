using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Price.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Price API",
                    Version = "v1",
                    Description = "RESTful API built with ASP.NET Core 3.1 to show restful services.",
                    Contact = new OpenApiContact
                    {
                        Name = "PTG",
                        Url = new Uri("https://www.ptgenergy.co.th/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = $"Started On: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}",
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
            //var path = (!env.IsProduction()) ? "" : "/price";
            app.UseSwagger().UseSwaggerUI(options =>
            {
                //options.SwaggerEndpoint($"{path}/swagger/v1/swagger.json", "Price API");
                //options.DocumentTitle = "Price API";

                options.SwaggerEndpoint($"{rootUrl}/swagger/v1/swagger.json", "Price API");
                options.DocumentTitle = $"Price API";
            });
            return app;
        }
    }
}
