using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Quartz;
using Vatno.Worker;
using Vatno.Worker.Extensions;
using Vatno.Worker.Healths;
using Vatno.Worker.Modules;
using Vatno.Worker.Setting;
using AzureStorageSettings = Vatno.Worker.BlobStorage.AzureStorageSettings;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration as IConfiguration;
var services = builder.Services;

GlobalContext.Properties["ProductName"] = configuration.GetValue<string>("ProductName");

builder.Host.ConfigureLogging(options => { options.AddLog4Net("log4net.config"); });
builder.Host.ConfigureAppConfiguration((context, config) =>
{
    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
    config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
    config.AddEnvironmentVariables();
});

services.AddHealthChecks().AddDatabaseHealthCheck();

services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(15);
    options.Period = TimeSpan.FromSeconds(15);
});

services.RegisterAppSettings<AzureStorageSettings>(configuration, nameof(AzureStorageSettings));
services.RegisterAppSettings<CornJobSetting>(configuration, nameof(CornJobSetting));

builder.Host.RegisterModuleWithAutofac(configuration);

services.AddAutoMapper(typeof(Worker).Assembly);
services.AddLogging();

// services.AddHostedService<Worker>();
services.AddQuartz(options =>
{
    options.UseMicrosoftDependencyInjectionJobFactory();
    options.ScheduleJob<Worker>(trigger =>
    {
        var cornJobSetting = new CornJobSetting();
        configuration.GetSection(nameof(CornJobSetting)).Bind(cornJobSetting);

        trigger.WithIdentity("VatExportTrigger")
            .WithCronSchedule(cornJobSetting.CornTime, s => s.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById(cornJobSetting.TimeZone)))
            //.WithSimpleSchedule(s => s.WithIntervalInSeconds(30).WithRepeatCount(1))
            .WithDescription("Trigger a for export vat");
    });
});

services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });

var app = builder.Build();
app.MapHealthChecks("/api/health");
await app.RunAsync();