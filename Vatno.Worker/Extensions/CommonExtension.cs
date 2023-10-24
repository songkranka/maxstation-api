namespace Vatno.Worker.Extensions;

public static class CommonExtension
{
    /// <summary>
    /// Validate object is null. If true is null, false not null
    /// </summary>
    /// <param name="value">Object for validate is null</param>
    /// <returns>true, false</returns>
    public static bool IsNullable(this object? value) => value == null;

    /// <summary>
    /// Validate string is null or empty. If true is null/emtpy. false is not null/not empty
    /// </summary>
    /// <param name="value">string value</param>
    /// <returns>true, false</returns>
    public static bool IsEmpty(this string value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Validate guid is empty. If true is empty, false not empty.
    /// Empty guid value : '00000000-0000-0000-0000-000000000000'
    /// </summary>
    /// <param name="value">Guid data</param>
    /// <returns>true, false</returns>
    public static bool IsEmpty(this Guid value) => Guid.Empty == value;

    /// <summary>
    /// Convert string to Guid.
    /// </summary>
    /// <param name="value">string of guid</param>
    /// <returns></returns>
    public static Guid ToGuid(this string value)
    {
        if (value.IsEmpty())
        {
            throw new ArgumentNullException(nameof(value));
        }

        return Guid.Parse(value);
    }

    /// <summary>
    /// Convert string to guid.
    /// </summary>
    /// <param name="value">Value for convert to Guid</param>
    /// <returns></returns>
    public static Guid? ToTryGuid(this string value)
    {
        bool isConvert = Guid.TryParse(value, out var newId);
        return isConvert ? newId : new Guid?();
    }

    public static void RegisterAppSettings<TAppSettings>(this IServiceCollection services, IConfiguration configuration, string sectionName)
        where TAppSettings : class
    {
        services.Configure<TAppSettings>(configuration.GetSection(sectionName));
    }
}