using Vatno.Worker.Context;

namespace Vatno.Worker.Healths;

public static class HealthCheckExtension
{
    public static IHealthChecksBuilder AddDatabaseHealthCheck(this IHealthChecksBuilder builder)
        => builder.AddCheck<DatabaseContextHealthCheck>(nameof(PTMaxStationContext), tags: new[] { nameof(IPTMaxStationContext) });
}