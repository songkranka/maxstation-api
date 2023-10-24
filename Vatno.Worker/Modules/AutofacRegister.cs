using Autofac;
using Autofac.Extensions.DependencyInjection;
using Vatno.Worker.Repositories;
using Vatno.Worker.Services;
using BlobStorageService = Vatno.Worker.BlobStorage.BlobStorageService;

namespace Vatno.Worker.Modules;

public static class AutofacRegisterExtension
{
    public static IHostBuilder RegisterModuleWithAutofac(this IHostBuilder host, IConfiguration configuration)
    {
        host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        host.ConfigureContainer<ContainerBuilder>(builder =>
        {
            builder.RegisterModule(new DbContextModule(configuration));

            builder.RegisterType<VatNoRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<VatNoService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<BlobStorageService>().AsImplementedInterfaces().InstancePerLifetimeScope();

        });

        return host;
    }
}