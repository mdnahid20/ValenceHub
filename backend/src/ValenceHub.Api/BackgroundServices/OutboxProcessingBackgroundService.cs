using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ValenceHub.Persistence.Write.Outbox.Processors;

namespace ValenceHub.Api.BackgroundServices;

public sealed class OutboxProcessingBackgroundService : BackgroundService
{
    private static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(10);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessingBackgroundService> _logger;
    private readonly bool _enabled;
    private readonly TimeSpan _interval;

    public OutboxProcessingBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessingBackgroundService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        _enabled = configuration.GetValue("Outbox:Enabled", true);

        var intervalSeconds = configuration.GetValue<int?>("Outbox:IntervalSeconds");
        _interval = intervalSeconds is > 0 ? TimeSpan.FromSeconds(intervalSeconds.Value) : DefaultInterval;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_enabled)
        {
            _logger.LogInformation("Outbox processing is disabled.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var job = scope.ServiceProvider.GetRequiredService<OutboxProcessingJob>();

                await job.ExecuteAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox processing failed.");
            }

            try
            {
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }
}

