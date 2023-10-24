//using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MaxStation.Utility.Extensions
{
    public static class CommonExtension
    {
        /// <summary>
        /// Validate object is null. If true is null, false not null
        /// </summary>
        /// <param name="value">Object for validate is null</param>
        /// <returns>true, false</returns>
        public static bool IsNullable(this object? value) => value == null;

        //public static void RegisterAppSettings<TAppSettings>(this IServiceCollection services, IConfiguration configuration, string sectionName)
        //where TAppSettings : class
        //{
        //    services.Configure<TAppSettings>(configuration.GetSection(sectionName));
        //}
    }
}
