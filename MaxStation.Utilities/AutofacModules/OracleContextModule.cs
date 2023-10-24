using Autofac;
//using MaxStation.Entities.Contexts.OracleContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MaxStation.Utilities.AutofacModules;

public class OracleContextModule : Module
{
    public IConfiguration Configuration { get; set; }

    public OracleContextModule(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    //protected override void Load(ContainerBuilder builder)
    //{
    //    builder.Register((com, _) =>
    //        {
    //            var strOracleConnection = string.Empty;
    //            try
    //            {
    //                strOracleConnection = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Production
    //                    ? Environment.GetEnvironmentVariable("OracleDbConnection")
    //                    : Configuration.GetValue<string>("OracleConnection"); //change connect {DefaultConnection,DevelopConnection}                                        
    //            }
    //            catch (Exception ex)
    //            {
    //                Console.WriteLine(ex);
    //                throw;
    //            }

    //            return new OracleContext(strOracleConnection);
    //        }).As<IOracleContext>()
    //        .AsSelf()
    //        .InstancePerLifetimeScope();
    //}
}

public class OracleDbContextModule : Module
{
    public IConfiguration Configuration { get; set; }

    public OracleDbContextModule(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    //protected override void Load(ContainerBuilder builder)
    //{
    //    builder.Register((com, _) =>
    //    {
    //        string dbConnection;
    //        try
    //        {
    //            dbConnection = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Production
    //                ? Environment.GetEnvironmentVariable("OracleDbConnection")
    //                : Configuration.GetValue<string>("OracleConnection"); //change connect {DefaultConnection,DevelopConnection}                                        
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex);
    //            throw;
    //        }

    //        var dbContextOption = new DbContextOptionsBuilder<OracleDbContext>();
    //        return dbContextOption.UseOracle($"{dbConnection}Statement Cache Size=1", options => { options.CommandTimeout(60); }).Options;
    //    }).SingleInstance();

    //    builder.Register((com, _)
    //            => new OracleDbContext(com.Resolve<DbContextOptions<OracleDbContext>>())).As<IOracleDbContext>()
    //        .AsSelf()
    //        .InstancePerLifetimeScope();
    //}
}