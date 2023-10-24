using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transferdata.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Transferdata API",
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
            });
            return services;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, IHostEnvironment env)
        {
            var rootUrl = Environment.GetEnvironmentVariable("ROOT_URL");            
            app.UseSwagger().UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"{rootUrl}/swagger/v1/swagger.json", "Transferdata API");
                options.DocumentTitle = $"Transferdata API";
            });
            return app;
        }
    }
}
