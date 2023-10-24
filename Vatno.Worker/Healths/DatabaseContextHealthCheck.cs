using Microsoft.Extensions.Diagnostics.HealthChecks;
using Vatno.Worker.Context;

namespace Vatno.Worker.Healths;

public class DatabaseContextHealthCheck : IHealthCheck
{
    private readonly IPTMaxStationContext _dbContext;

    public DatabaseContextHealthCheck(IPTMaxStationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var isConnected = await _dbContext.Database.CanConnectAsync(cancellationToken);
        string description = "Database connection result";
        var values = new Dictionary<string, object> { { "DatbaseConnection", isConnected } };

        return new HealthCheckResult(isConnected ? HealthStatus.Healthy : HealthStatus.Unhealthy, description, null, values);
    }
}