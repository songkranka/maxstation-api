using Autofac;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Vatno.Worker.Context;

namespace Vatno.Worker.Modules;

public class DbContextModule : Module
{
    public IConfiguration Configuration { get; }

    public DbContextModule(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register((com, _) =>
        {
            var connectionString = string.Empty;
            //-------- Kuber ------------
            var connectionFile = Environment.GetEnvironmentVariable("ConnectionFile");
            if (connectionFile != null)
            {
                FileStream fileStream = new FileStream(connectionFile, FileMode.Open);
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    connectionString = reader.ReadLine();
                }
            }
            else
            {
                try
                {
                    //var KeyVault = Environment.GetEnvironmentVariable("KeyVault");
                   // var SecretKey = Environment.GetEnvironmentVariable("SecretKey");
                    var KeyVault = Configuration.GetValue<string>("KeyVault");
                    var SecretKey = Configuration.GetValue<string>("SecretKey");
                    if (KeyVault != null)
                    {
                        var secretClient = new SecretClient(new Uri(KeyVault), new DefaultAzureCredential());
                        KeyVaultSecret secret = secretClient.GetSecret(SecretKey);
                        connectionString = secret.Value;
                    }

                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != Environments.Production)
                    {
                        connectionString = Configuration.GetConnectionString("DefaultConnection"); //change conect {DefaultConnection,DevelopConnection}
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            var dbContextOption = new DbContextOptionsBuilder<PTMaxStationContext>();
            return dbContextOption.UseSqlServer(connectionString, optionsBuilder =>
            {
                optionsBuilder.EnableRetryOnFailure(20);
                optionsBuilder.MigrationsAssembly(typeof(PTMaxStationContext).Assembly.GetName().Name);
                optionsBuilder.CommandTimeout(9000);
            }).Options;
        }).SingleInstance();

        builder.Register((com, _) =>
            {
                var contextOptions = com.Resolve<DbContextOptions<PTMaxStationContext>>();

                return new PTMaxStationContext(contextOptions);
            }).AsImplementedInterfaces()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}