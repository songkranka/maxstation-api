using Autofac;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Vatno.Worker.Context;

//using MaxStation.Entities.Contexts.SQLContext;

namespace Vatno.Worker.Modules;

public class SqlContextModule : Module
{
    public IConfiguration Configuration { get; }

    public SqlContextModule(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register((com, _) =>
        {
            var dbconnection = string.Empty;
            //-------- Kuber ------------
            var ConnectionFile = Environment.GetEnvironmentVariable("ConnectionFile");
            if (ConnectionFile != null)
            {
                FileStream fileStream = new FileStream(ConnectionFile, FileMode.Open);
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    dbconnection = reader.ReadLine();
                }
            }
            else
            {
                try
                {
                    var KeyVault = Environment.GetEnvironmentVariable("KeyVault");
                    var SecretKey = Environment.GetEnvironmentVariable("SecretKey");
                    if (KeyVault != null)
                    {
                        var secretClient = new SecretClient(new Uri(KeyVault), new DefaultAzureCredential());
                        KeyVaultSecret secret = secretClient.GetSecret(SecretKey);
                        dbconnection = secret.Value;
                    }

                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != Environments.Production)
                    {
                        dbconnection = Configuration.GetValue<string>("SQLConnection"); //change conect {DefaultConnection,DevelopConnection}
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            var dbContextOption = new DbContextOptionsBuilder<PTMaxStationContext>();
            return dbContextOption.UseSqlServer(dbconnection, options =>
            {
                options.EnableRetryOnFailure(20);
                options.MigrationsAssembly(typeof(PTMaxStationContext).Assembly.GetName().Name);
                options.CommandTimeout(9000);
            }).Options;
        }).SingleInstance();
    }
}