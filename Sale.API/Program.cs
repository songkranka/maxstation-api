using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sale.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            //CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging(builder => {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddLog4Net("log4net.config");
                });

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //    .ConfigureAppConfiguration((context, config) =>
        //    {
        //        var root = config.Build();
        //        var vault = root["KeyVault:Vault"];
        //        if (!string.IsNullOrEmpty(vault))
        //        {
        //            config.AddAzureKeyVault(
        //            $"https://{root["KeyVault:Vault"]}.vault.azure.net/",
        //            root["KeyVault:ClientId"],
        //            root["KeyVault:ClientSecret"]);
        //        }
        //    })
        //    .UseStartup<Startup>();

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //    .ConfigureAppConfiguration((context, config) =>
        //    {
        //        if (context.HostingEnvironment.IsProduction() || context.HostingEnvironment.IsStaging())
        //        {
        //            var root = config.Build();
        //            var secretClient = new SecretClient(
        //                    new Uri($"https://{root["KeyVault:KeyVaultName"]}.vault.azure.net/"),
        //                    new DefaultAzureCredential());
        //            config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
        //        }
        //    })
        //    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
