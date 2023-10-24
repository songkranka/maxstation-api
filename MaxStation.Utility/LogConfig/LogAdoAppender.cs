using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using log4net.Config;
using log4net.Repository;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Linq;
using MaxStation.Utility.Extensions;

namespace MaxStation.Utility.LogConfig
{
    public static class LogConfiguration
    {
        public static void Log4NetAdoAppenderRegister(this IServiceCollection services, IConfiguration configuration)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load((Stream)File.OpenRead("log4net.config"));
            ILoggerRepository repository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            XmlConfigurator.Configure(repository, xmlDocument["log4net"]);


            IAppender[] source = !repository.IsNullable() ? repository.GetAppenders() : throw new Exception("Cannot set configure of connection because respository is null.");
            if (source.Length == 0) throw new Exception("Cannot set configure of connection because appender no config.");
            foreach (IAppender appender in ((IEnumerable<IAppender>)source).Where<IAppender>((Func<IAppender, bool>)(w => w.GetType() == typeof(MicroKnights.Logging.AdoNetAppender))))
            {
                MicroKnights.Logging.AdoNetAppender adoNetAppender = appender as MicroKnights.Logging.AdoNetAppender;
                if (!adoNetAppender.IsNullable())
                {
                    adoNetAppender.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                    adoNetAppender.ActivateOptions();
                }
            }
        }
    }
}